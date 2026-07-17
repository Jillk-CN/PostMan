using System;

/// <summary>
/// VHS 滤镜参数快照（可序列化值类型）。
/// 在 VHSPresetSO.settings 中于 Inspector 直接编辑；运行时只读。
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
    // 默认预设（供 VHSPresetSO.settings 字段初始化用）
    // ─────────────────────────────────────────────

    /// <summary>返回开启常用效果的默认参数快照，供 Inspector 字段初始化。</summary>
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

}
