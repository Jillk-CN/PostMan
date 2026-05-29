using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// VHS 滤镜 UI 控制层。
/// 持有 VHSFilterEffect 和 VHSVolumeController 的引用，
/// 接收 Slider / Toggle 输入，构造不可变 VHSSettings 并推送给两个控制器。
/// 同时负责将当前参数值回写到 UI 组件，保证显示与效果一致。
/// </summary>
public sealed class VHSFilterController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 核心引用
    // ─────────────────────────────────────────────

    [Header("核心引用")]
    [Tooltip("URP Renderer Feature 中的 VHSFilterEffect 实例")]
    [SerializeField] private VHSFilterEffect _effect;

    [Tooltip("场景中的 VHSVolumeController 组件")]
    [SerializeField] private VHSVolumeController _volumeCtrl;

    // ─────────────────────────────────────────────
    // UI 引用：Bloom
    // ─────────────────────────────────────────────

    [Header("UI — Bloom")]
    [SerializeField] private Toggle _bloomEnabledToggle;
    [SerializeField] private Slider _bloomRadiusSlider;
    [SerializeField] private Slider _bloomIntensitySlider;
    [SerializeField] private Slider _bloomThresholdSlider;

    // ─────────────────────────────────────────────
    // UI 引用：Chromatic Aberration
    // ─────────────────────────────────────────────

    [Header("UI — Chromatic Aberration")]
    [SerializeField] private Toggle _chromaticEnabledToggle;
    [SerializeField] private Slider _chromaticStrengthSlider;

    // ─────────────────────────────────────────────
    // UI 引用：Vignette
    // ─────────────────────────────────────────────

    [Header("UI — Vignette")]
    [SerializeField] private Toggle _vignetteEnabledToggle;
    [SerializeField] private Slider _vignetteStrengthSlider;
    [SerializeField] private Slider _vignetteIntensitySlider;

    // ─────────────────────────────────────────────
    // UI 引用：Noise
    // ─────────────────────────────────────────────

    [Header("UI — Noise")]
    [SerializeField] private Toggle _noiseEnabledToggle;
    [SerializeField] private Slider _noiseStrengthSlider;

    // ─────────────────────────────────────────────
    // UI 引用：VHS Distortion
    // ─────────────────────────────────────────────

    [Header("UI — VHS Distortion")]
    [SerializeField] private Toggle _distortionEnabledToggle;
    [SerializeField] private Slider _distortionStrengthSlider;

    // ─────────────────────────────────────────────
    // UI 引用：Scanlines
    // ─────────────────────────────────────────────

    [Header("UI — Scanlines")]
    [SerializeField] private Toggle _scanlinesEnabledToggle;
    [SerializeField] private Slider _scanlinesStrengthSlider;

    // ─────────────────────────────────────────────
    // UI 引用：CRT Curve
    // ─────────────────────────────────────────────

    [Header("UI — CRT Curve")]
    [SerializeField] private Toggle _crtEnabledToggle;
    [SerializeField] private Slider _crtCurveSlider;

    // ─────────────────────────────────────────────
    // 运行时状态
    // ─────────────────────────────────────────────

    // 当前参数快照（不可变，每次修改返回新实例）
    private VHSSettings _settings = VHSSettings.Default;

    // ─────────────────────────────────────────────
    // Unity 生命周期
    // ─────────────────────────────────────────────

    private void Start()
    {
        // 启动时将默认参数推送给 Effect，并同步 UI 显示值
        Push(_settings);
    }

    // ─────────────────────────────────────────────
    // 公开接口：Bloom
    // ─────────────────────────────────────────────

    /// <summary>设置 Bloom 开关。绑定到 Toggle.OnValueChanged。</summary>
    public void SetBloomEnabled(bool v) => Push(_settings.WithBloomEnabled(v));

    /// <summary>设置 Bloom 扩散半径 [0, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetBloomRadius(float v) => Push(_settings.WithBloomRadius(v));

    /// <summary>设置 Bloom 强度 [0, 10]。绑定到 Slider.OnValueChanged。</summary>
    public void SetBloomIntensity(float v) => Push(_settings.WithBloomIntensity(v));

    /// <summary>设置 Bloom 阈值 [0, 3]。绑定到 Slider.OnValueChanged。</summary>
    public void SetBloomThreshold(float v) => Push(_settings.WithBloomThreshold(v));

    // ─────────────────────────────────────────────
    // 公开接口：Chromatic Aberration
    // ─────────────────────────────────────────────

    /// <summary>设置色差开关。绑定到 Toggle.OnValueChanged。</summary>
    public void SetChromaticEnabled(bool v) => Push(_settings.WithChromaticEnabled(v));

    /// <summary>设置色差强度 [0, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetChromaticStrength(float v) => Push(_settings.WithChromaticStrength(v));

    // ─────────────────────────────────────────────
    // 公开接口：Vignette
    // ─────────────────────────────────────────────

    /// <summary>设置暗角开关。绑定到 Toggle.OnValueChanged。</summary>
    public void SetVignetteEnabled(bool v) => Push(_settings.WithVignetteEnabled(v));

    /// <summary>设置暗角平滑度 [0.01, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetVignetteStrength(float v) => Push(_settings.WithVignetteStrength(v));

    /// <summary>设置暗角强度 [0, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetVignetteIntensity(float v) => Push(_settings.WithVignetteIntensity(v));

    // ─────────────────────────────────────────────
    // 公开接口：Noise
    // ─────────────────────────────────────────────

    /// <summary>设置噪点开关。绑定到 Toggle.OnValueChanged。</summary>
    public void SetNoiseEnabled(bool v) => Push(_settings.WithNoiseEnabled(v));

    /// <summary>设置噪点强度 [0, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetNoiseStrength(float v) => Push(_settings.WithNoiseStrength(v));

    // ─────────────────────────────────────────────
    // 公开接口：VHS Distortion
    // ─────────────────────────────────────────────

    /// <summary>设置 VHS 扭曲开关。绑定到 Toggle.OnValueChanged。</summary>
    public void SetDistortionEnabled(bool v) => Push(_settings.WithDistortionEnabled(v));

    /// <summary>设置 VHS 扭曲强度 [0, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetDistortionStrength(float v) => Push(_settings.WithDistortionStrength(v));

    // ─────────────────────────────────────────────
    // 公开接口：Scanlines
    // ─────────────────────────────────────────────

    /// <summary>设置扫描线开关。绑定到 Toggle.OnValueChanged。</summary>
    public void SetScanlinesEnabled(bool v) => Push(_settings.WithScanlinesEnabled(v));

    /// <summary>设置扫描线强度 [0, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetScanlinesStrength(float v) => Push(_settings.WithScanlinesStrength(v));

    // ─────────────────────────────────────────────
    // 公开接口：CRT Curve
    // ─────────────────────────────────────────────

    /// <summary>设置 CRT 弯曲开关。绑定到 Toggle.OnValueChanged。</summary>
    public void SetCRTEnabled(bool v) => Push(_settings.WithCRTEnabled(v));

    /// <summary>设置 CRT 弯曲曲率 [0, 1]。绑定到 Slider.OnValueChanged。</summary>
    public void SetCRTCurve(float v) => Push(_settings.WithCRTCurve(v));

    // ─────────────────────────────────────────────
    // 私有：统一推送参数
    // ─────────────────────────────────────────────

    /// <summary>
    /// 更新内部快照，推送给两个控制器，并将新值回写到 UI 组件。
    /// 使用 SetIsOnWithoutNotify / SetValueWithoutNotify 避免触发 OnValueChanged 循环。
    /// </summary>
    private void Push(VHSSettings newSettings)
    {
        _settings = newSettings;
        _effect?.Apply(_settings);
        _volumeCtrl?.Apply(_settings);
        SyncUI();
    }

    /// <summary>
    /// 在面板 SetActive(true) 之前调用，确保所有 Toggle 的 m_IsOn 与当前 _settings 一致。
    /// 这样 Toggle OnEnable 触发 onValueChanged 时传入的值与 _settings 相同，Push 不会改变状态。
    /// </summary>
    public void SyncBeforeShow() => SyncUI();

    /// <summary>
    /// 将当前参数快照的值回写到所有已绑定的 UI 组件。
    /// 未绑定（null）的字段自动跳过，不会报错。
    /// </summary>
    private void SyncUI()
    {
        // Bloom
        _bloomEnabledToggle?.SetIsOnWithoutNotify(_settings.bloomEnabled);
        _bloomRadiusSlider?.SetValueWithoutNotify(_settings.bloomRadius);
        _bloomIntensitySlider?.SetValueWithoutNotify(_settings.bloomIntensity);
        _bloomThresholdSlider?.SetValueWithoutNotify(_settings.bloomThreshold);

        // Chromatic Aberration
        _chromaticEnabledToggle?.SetIsOnWithoutNotify(_settings.chromaticEnabled);
        _chromaticStrengthSlider?.SetValueWithoutNotify(_settings.chromaticStrength);

        // Vignette
        _vignetteEnabledToggle?.SetIsOnWithoutNotify(_settings.vignetteEnabled);
        _vignetteStrengthSlider?.SetValueWithoutNotify(_settings.vignetteStrength);
        _vignetteIntensitySlider?.SetValueWithoutNotify(_settings.vignetteIntensity);

        // Noise
        _noiseEnabledToggle?.SetIsOnWithoutNotify(_settings.noiseEnabled);
        _noiseStrengthSlider?.SetValueWithoutNotify(_settings.noiseStrength);

        // VHS Distortion
        _distortionEnabledToggle?.SetIsOnWithoutNotify(_settings.distortionEnabled);
        _distortionStrengthSlider?.SetValueWithoutNotify(_settings.distortionStrength);

        // Scanlines
        _scanlinesEnabledToggle?.SetIsOnWithoutNotify(_settings.scanlinesEnabled);
        _scanlinesStrengthSlider?.SetValueWithoutNotify(_settings.scanlinesStrength);

        // CRT Curve
        _crtEnabledToggle?.SetIsOnWithoutNotify(_settings.crtEnabled);
        _crtCurveSlider?.SetValueWithoutNotify(_settings.crtCurve);
    }
}
