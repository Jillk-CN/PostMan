// VHS 滤镜 Blit Shader（URP 14）
// 实现 Noise / VHS Distortion / Scanlines / CRT Curve 四种效果
// Bloom / Chromatic Aberration / Vignette 由 URP Volume 系统处理，此 Shader 不涉及
Shader "PostMan/VHSFilter"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "VHSFilter_Main"

            HLSLPROGRAM
            #pragma vertex   Vert      // 使用 Blit.hlsl 提供的标准顶点着色器
            #pragma fragment FragVHS

            // URP Core.hlsl 必须在 Blit.hlsl 之前 include，提供 TEXTURE2D_X 等宏定义
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // ─────────────────────────────────────────────
            // 参数声明（由 VHSFilterEffect 通过 Material.SetFloat 传入）
            // ─────────────────────────────────────────────

            float _NoiseEnabled;
            float _NoiseStrength;
            float _DistortEnabled;
            float _DistortStrength;
            float _ScanlinesEnabled;
            float _ScanlinesStrength;
            float _CRTEnabled;
            float _CRTCurve;

            // ─────────────────────────────────────────────
            // 工具函数
            // ─────────────────────────────────────────────

            /// 伪随机哈希函数（Gold Noise 变体），输入 2D 坐标，输出 [0, 1]
            float Hash(float2 p)
            {
                p = frac(p * float2(443.897, 441.423));
                p += dot(p, p.yx + 19.19);
                return frac((p.x + p.y) * p.x);
            }

            // ─────────────────────────────────────────────
            // 效果函数
            // ─────────────────────────────────────────────

            /// CRT 桶形畸变：将 uv 从 [0,1] 映射到 [-1,1] 后施加桶形变形，返回变形后的 uv
            float2 ApplyCRTCurve(float2 uv, float curve)
            {
                float2 cc = uv * 2.0 - 1.0;                        // 映射到 [-1, 1]
                cc.x *= 1.0 + cc.y * cc.y * curve * 0.3;           // x 轴随 y² 弯曲
                cc.y *= 1.0 + cc.x * cc.x * curve * 0.3;           // y 轴随 x² 弯曲
                return cc * 0.5 + 0.5;                              // 回到 [0, 1]
            }

            /// VHS 水平错位扭曲：对每个横向块施加随机水平偏移，模拟磁带抖动
            float2 ApplyDistortion(float2 uv, float strength)
            {
                float blockY    = floor(uv.y * 20.0);               // 将屏幕纵向分 20 块
                float timeSlice = floor(_Time.y * 10.0);            // 每 0.1s 更新一次随机种子

                // 每块的随机水平偏移，映射到 [-1, 1]
                float jitter = (Hash(float2(blockY, timeSlice)) - 0.5) * 2.0;
                uv.x += jitter * strength * 0.05;

                // 概率性撕裂线：在随机 y 位置产生大幅水平偏移
                float tearY    = Hash(float2(timeSlice, 0.0));
                float tearDist = abs(uv.y - tearY);
                if (tearDist < 0.005)
                    uv.x += (Hash(float2(uv.y, timeSlice + 1.0)) - 0.5) * strength * 0.3;

                return uv;
            }

            /// CRT 扫描线：用 sin 函数生成周期性暗带，叠加到颜色上
            float3 ApplyScanlines(float3 color, float2 uv, float strength)
            {
                // 频率对应约 540 条线（1080p 下每两像素一条）
                float scanline = sin(uv.y * 1080.0 * 3.14159265) * 0.5 + 0.5;
                // mask 范围 [1 - strength*0.3, 1]，避免全黑
                float mask = 1.0 - strength * 0.3 * (1.0 - scanline);
                return color * mask;
            }

            /// 胶片噪点：每帧生成随机颗粒，以 Overlay 混合模式叠加到原色
            float3 ApplyNoise(float3 color, float2 uv, float strength)
            {
                // 用 uv + 时间偏移作为种子，保证每帧不同
                float grain = Hash(uv + frac(_Time.y * 0.1));

                // Overlay 混合：亮区提亮，暗区压暗，保留色调
                float3 overlay = grain < 0.5
                    ? 2.0 * color * grain
                    : 1.0 - 2.0 * (1.0 - color) * (1.0 - grain);

                return lerp(color, overlay, strength);
            }

            // ─────────────────────────────────────────────
            // 主片元着色器
            // ─────────────────────────────────────────────

            float4 FragVHS(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.texcoord;

                // Step 1：CRT 弯曲变形（最先处理，影响后续所有采样坐标）
                if (_CRTEnabled > 0.5)
                {
                    uv = ApplyCRTCurve(uv, _CRTCurve);
                    // 超出屏幕范围的区域输出黑色（模拟 CRT 边缘黑框）
                    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                        return float4(0.0, 0.0, 0.0, 1.0);
                }

                // Step 2：VHS 水平错位扭曲（在采样前修改 uv）
                if (_DistortEnabled > 0.5)
                    uv = ApplyDistortion(uv, _DistortStrength);

                // Step 3：采样原始颜色（使用变形后的 uv）
                float3 color = SAMPLE_TEXTURE2D_X_LOD(_BlitTexture, sampler_LinearClamp, uv, 0).rgb;

                // Step 4：CRT 扫描线（叠加到采样颜色上）
                if (_ScanlinesEnabled > 0.5)
                    color = ApplyScanlines(color, uv, _ScanlinesStrength);

                // Step 5：胶片噪点（最后叠加，保留其他效果的细节）
                if (_NoiseEnabled > 0.5)
                    color = ApplyNoise(color, uv, _NoiseStrength);

                return float4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
