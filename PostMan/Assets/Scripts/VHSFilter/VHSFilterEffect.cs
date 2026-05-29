using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// 注意：ScriptableRendererFeature 不能放在 namespace 内，否则无法出现在 Add Renderer Feature 菜单中

/// <summary>
/// VHS 滤镜核心：ScriptableRendererFeature。
/// 管理 Material 生命周期，接收来自控制层的参数快照，传递给 Pass 和 Shader。
/// 需要在 URP Renderer Asset（如 URP-HighFidelity-Renderer）中手动添加此 Feature。
/// </summary>
public class VHSFilterEffect : ScriptableRendererFeature
{
    // ─────────────────────────────────────────────
    // Inspector 配置
    // ─────────────────────────────────────────────

    [Header("Shader 引用")]
    [Tooltip("拖入 Assets/Shaders/VHSFilter.shader")]
    [SerializeField] private Shader _vhsShader;

    [Header("注入时机")]
    [Tooltip("渲染管线中的注入位置，默认在后处理之前")]
    [SerializeField] private RenderPassEvent _passEvent = RenderPassEvent.BeforeRenderingPostProcessing;

    // ─────────────────────────────────────────────
    // 运行时状态
    // ─────────────────────────────────────────────

    private VHSFilterPass _pass;
    private Material _material;
    private VHSSettings _currentSettings = VHSSettings.Default;

    // ─────────────────────────────────────────────
    // ScriptableRendererFeature 生命周期
    // ─────────────────────────────────────────────

    /// <summary>创建 Material 和 Pass 实例。</summary>
    public override void Create()
    {
        if (_vhsShader == null) return;

        _material = CoreUtils.CreateEngineMaterial(_vhsShader);
        _pass = new VHSFilterPass(_material, _passEvent);
    }

    /// <summary>
    /// 将 Pass 加入渲染队列。仅在 Game 相机上执行，跳过 Scene View / Preview。
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer,
                                         ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        if (_pass == null || _material == null) return;

        renderer.EnqueuePass(_pass);
    }

    /// <summary>将相机颜色 RT 传递给 Pass。</summary>
    public override void SetupRenderPasses(ScriptableRenderer renderer,
                                           in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game) return;
        if (_pass == null) return;

        _pass.SetSource(renderer.cameraColorTargetHandle);
    }

    /// <summary>释放 Material 和 Pass 资源。</summary>
    protected override void Dispose(bool disposing)
    {
        _pass?.Dispose();
        CoreUtils.Destroy(_material);
    }

    // ─────────────────────────────────────────────
    // 公开接口（供 VHSFilterController 调用）
    // ─────────────────────────────────────────────

    /// <summary>
    /// 应用一组新的 VHS 参数快照。
    /// 此方法是 Controller 与 Effect 之间的唯一接口。
    /// </summary>
    /// <param name="settings">新的参数快照。</param>
    public void Apply(in VHSSettings settings)
    {
        _currentSettings = settings;
        if (_material == null) return;
        UploadToMaterial(settings);
    }

    /// <summary>获取当前参数快照（只读）。</summary>
    public VHSSettings CurrentSettings => _currentSettings;

    // ─────────────────────────────────────────────
    // 私有：将参数写入 Material
    // ─────────────────────────────────────────────

    /// <summary>将自定义 Shader 效果的开关和强度值写入 Material。</summary>
    private void UploadToMaterial(in VHSSettings s)
    {
        _material.SetFloat(ShaderIDs.NoiseEnabled, s.noiseEnabled ? 1f : 0f);
        _material.SetFloat(ShaderIDs.NoiseStrength, s.noiseStrength);
        _material.SetFloat(ShaderIDs.DistortEnabled, s.distortionEnabled ? 1f : 0f);
        _material.SetFloat(ShaderIDs.DistortStrength, s.distortionStrength);
        _material.SetFloat(ShaderIDs.ScanlinesEnabled, s.scanlinesEnabled ? 1f : 0f);
        _material.SetFloat(ShaderIDs.ScanlinesStrength, s.scanlinesStrength);
        _material.SetFloat(ShaderIDs.CRTEnabled, s.crtEnabled ? 1f : 0f);
        _material.SetFloat(ShaderIDs.CRTCurve, s.crtCurve);
    }

    // ─────────────────────────────────────────────
    // Shader Property ID 缓存（避免字符串查找开销）
    // ─────────────────────────────────────────────

    private static class ShaderIDs
    {
        public static readonly int NoiseEnabled = Shader.PropertyToID("_NoiseEnabled");
        public static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        public static readonly int DistortEnabled = Shader.PropertyToID("_DistortEnabled");
        public static readonly int DistortStrength = Shader.PropertyToID("_DistortStrength");
        public static readonly int ScanlinesEnabled = Shader.PropertyToID("_ScanlinesEnabled");
        public static readonly int ScanlinesStrength = Shader.PropertyToID("_ScanlinesStrength");
        public static readonly int CRTEnabled = Shader.PropertyToID("_CRTEnabled");
        public static readonly int CRTCurve = Shader.PropertyToID("_CRTCurve");
    }
}
