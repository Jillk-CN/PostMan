using System;

/// <summary>
/// VHS 滤镜参数快照（不可变值类型）。
/// 遵循项目不可变原则：修改时通过 With* 方法返回新实例，原实例保持不变。
/// </summary>
[Serializable]
public struct VHSSettings
{
    // ─────────────────────────────────────────────
    // Bloom（复用 URP Volume）
    // ─────────────────────────────────────────────

    /// <summary>是否启用 Bloom 效果。</summary>
    public bool bloomEnabled;

    /// <summary>Bloom 扩散半径，映射到 URP Bloom.scatter，范围 [0, 1]。</summary>
    public float bloomRadius;

    /// <summary>Bloom 强度，范围 [0, 10]。</summary>
    public float bloomIntensity;

    /// <summary>Bloom 亮度阈值，范围 [0, 3]。</summary>
    public float bloomThreshold;

    // ─────────────────────────────────────────────
    // Chromatic Aberration（复用 URP Volume）
    // ─────────────────────────────────────────────

    /// <summary>是否启用色差效果。</summary>
    public bool chromaticEnabled;

    /// <summary>色差强度，范围 [0, 1]。</summary>
    public float chromaticStrength;

    // ─────────────────────────────────────────────
    // Vignette（复用 URP Volume）
    // ─────────────────────────────────────────────

    /// <summary>是否启用暗角效果。</summary>
    public bool vignetteEnabled;

    /// <summary>暗角平滑度，映射到 URP Vignette.smoothness，范围 [0.01, 1]。</summary>
    public float vignetteStrength;

    /// <summary>暗角强度，范围 [0, 1]。</summary>
    public float vignetteIntensity;

    // ─────────────────────────────────────────────
    // Noise（自定义 Shader）
    // ─────────────────────────────────────────────

    /// <summary>是否启用胶片噪点效果。</summary>
    public bool noiseEnabled;

    /// <summary>噪点强度，范围 [0, 1]。</summary>
    public float noiseStrength;

    // ─────────────────────────────────────────────
    // VHS Distortion（自定义 Shader）
    // ─────────────────────────────────────────────

    /// <summary>是否启用 VHS 水平错位扭曲效果。</summary>
    public bool distortionEnabled;

    /// <summary>扭曲强度，范围 [0, 1]。</summary>
    public float distortionStrength;

    // ─────────────────────────────────────────────
    // Scanlines（自定义 Shader）
    // ─────────────────────────────────────────────

    /// <summary>是否启用 CRT 扫描线效果。</summary>
    public bool scanlinesEnabled;

    /// <summary>扫描线强度，范围 [0, 1]。</summary>
    public float scanlinesStrength;

    // ─────────────────────────────────────────────
    // CRT TV（自定义 Shader）
    // ─────────────────────────────────────────────

    /// <summary>是否启用 CRT 屏幕弯曲效果。</summary>
    public bool crtEnabled;

    /// <summary>CRT 桶形畸变曲率，范围 [0, 1]。</summary>
    public float crtCurve;

    // ─────────────────────────────────────────────
    // 默认预设
    // ─────────────────────────────────────────────

    /// <summary>返回所有效果关闭的默认参数快照。</summary>
    public static VHSSettings Default => new VHSSettings
    {
        bloomEnabled       = false,
        bloomRadius        = 0.7f,
        bloomIntensity     = 1f,
        bloomThreshold     = 1f,
        chromaticEnabled   = false,
        chromaticStrength  = 0f,
        vignetteEnabled    = false,
        vignetteStrength   = 0.4f,
        vignetteIntensity  = 0.25f,
        noiseEnabled       = false,
        noiseStrength      = 0f,
        distortionEnabled  = false,
        distortionStrength = 0f,
        scanlinesEnabled   = false,
        scanlinesStrength  = 0f,
        crtEnabled         = false,
        crtCurve           = 0f,
    };

    // ─────────────────────────────────────────────
    // 不可变更新方法（返回新实例）
    // ─────────────────────────────────────────────

    /// <summary>返回更新了 bloomEnabled 的新快照。</summary>
    public VHSSettings WithBloomEnabled(bool v)
    { var s = this; s.bloomEnabled = v; return s; }

    /// <summary>返回更新了 bloomRadius 的新快照。</summary>
    public VHSSettings WithBloomRadius(float v)
    { var s = this; s.bloomRadius = v; return s; }

    /// <summary>返回更新了 bloomIntensity 的新快照。</summary>
    public VHSSettings WithBloomIntensity(float v)
    { var s = this; s.bloomIntensity = v; return s; }

    /// <summary>返回更新了 bloomThreshold 的新快照。</summary>
    public VHSSettings WithBloomThreshold(float v)
    { var s = this; s.bloomThreshold = v; return s; }

    /// <summary>返回更新了 chromaticEnabled 的新快照。</summary>
    public VHSSettings WithChromaticEnabled(bool v)
    { var s = this; s.chromaticEnabled = v; return s; }

    /// <summary>返回更新了 chromaticStrength 的新快照。</summary>
    public VHSSettings WithChromaticStrength(float v)
    { var s = this; s.chromaticStrength = v; return s; }

    /// <summary>返回更新了 vignetteEnabled 的新快照。</summary>
    public VHSSettings WithVignetteEnabled(bool v)
    { var s = this; s.vignetteEnabled = v; return s; }

    /// <summary>返回更新了 vignetteStrength 的新快照。</summary>
    public VHSSettings WithVignetteStrength(float v)
    { var s = this; s.vignetteStrength = v; return s; }

    /// <summary>返回更新了 vignetteIntensity 的新快照。</summary>
    public VHSSettings WithVignetteIntensity(float v)
    { var s = this; s.vignetteIntensity = v; return s; }

    /// <summary>返回更新了 noiseEnabled 的新快照。</summary>
    public VHSSettings WithNoiseEnabled(bool v)
    { var s = this; s.noiseEnabled = v; return s; }

    /// <summary>返回更新了 noiseStrength 的新快照。</summary>
    public VHSSettings WithNoiseStrength(float v)
    { var s = this; s.noiseStrength = v; return s; }

    /// <summary>返回更新了 distortionEnabled 的新快照。</summary>
    public VHSSettings WithDistortionEnabled(bool v)
    { var s = this; s.distortionEnabled = v; return s; }

    /// <summary>返回更新了 distortionStrength 的新快照。</summary>
    public VHSSettings WithDistortionStrength(float v)
    { var s = this; s.distortionStrength = v; return s; }

    /// <summary>返回更新了 scanlinesEnabled 的新快照。</summary>
    public VHSSettings WithScanlinesEnabled(bool v)
    { var s = this; s.scanlinesEnabled = v; return s; }

    /// <summary>返回更新了 scanlinesStrength 的新快照。</summary>
    public VHSSettings WithScanlinesStrength(float v)
    { var s = this; s.scanlinesStrength = v; return s; }

    /// <summary>返回更新了 crtEnabled 的新快照。</summary>
    public VHSSettings WithCRTEnabled(bool v)
    { var s = this; s.crtEnabled = v; return s; }

    /// <summary>返回更新了 crtCurve 的新快照。</summary>
    public VHSSettings WithCRTCurve(float v)
    { var s = this; s.crtCurve = v; return s; }
}
