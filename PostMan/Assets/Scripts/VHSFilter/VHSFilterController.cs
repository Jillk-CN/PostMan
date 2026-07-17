using PostMan.Common;
using UnityEngine;

/// <summary>
/// VHS 滤镜运行时控制器（MonoSingleton）。
/// 所有效果参数由 Inspector 中的 VHSPresetSO 配置，游戏内只保留总开/关。
/// </summary>
public sealed class VHSFilterController : MonoSingleton<VHSFilterController>
{
    // ─────────────────────────────────────────────
    // Inspector 引用
    // ─────────────────────────────────────────────

    [Header("核心引用")]
    [Tooltip("URP Renderer Feature 中的 VHSFilterEffect 实例")]
    [SerializeField] private VHSFilterEffect _effect;

    [Tooltip("场景中的 VHSVolumeController 组件")]
    [SerializeField] private VHSVolumeController _volumeCtrl;

    [Header("预设")]
    [Tooltip("包含所有效果参数的 ScriptableObject，在 Inspector 中配置")]
    [SerializeField] private VHSPresetSO _preset;

    // ─────────────────────────────────────────────
    // 运行时状态
    // ─────────────────────────────────────────────

    private bool _isEnabled;

    /// <summary>当前 VHS 滤镜是否启用。</summary>
    public bool IsEnabled => _isEnabled;

    // ─────────────────────────────────────────────
    // MonoSingleton 初始化
    // ─────────────────────────────────────────────

    protected override void Init()
    {
        _isEnabled = _preset != null && _preset.enabledByDefault;
        Apply();
    }

    // ─────────────────────────────────────────────
    // 公开 API
    // ─────────────────────────────────────────────

    /// <summary>设置 VHS 滤镜开关状态。由 SettingsPanel toggleVHS 回调。</summary>
    public void SetEnabled(bool value)
    {
        _isEnabled = value;
        Apply();
    }

    /// <summary>切换 VHS 滤镜开关状态。由 VHSToggleInputSource 键盘快捷键调用。</summary>
    public void Toggle()
    {
        _isEnabled = !_isEnabled;
        Apply();
    }

    // ─────────────────────────────────────────────
    // 私有：推送参数
    // ─────────────────────────────────────────────

    /// <summary>
    /// 根据 _isEnabled 将 SO 参数或全关快照推送给 _effect 和 _volumeCtrl。
    /// </summary>
    private void Apply()
    {
        VHSSettings snapshot = _isEnabled && _preset != null
            ? _preset.settings
            : DisabledSnapshot();

        _effect?.Apply(snapshot);
        _volumeCtrl?.Apply(snapshot);
    }

    /// <summary>返回所有效果关闭时的参数快照（_isEnabled=false 时使用）。</summary>
    private static VHSSettings DisabledSnapshot() => new VHSSettings
    {
        bloomEnabled      = false,
        chromaticEnabled  = false,
        vignetteEnabled   = false,
        noiseEnabled      = false,
        distortionEnabled = false,
        scanlinesEnabled  = false,
        crtEnabled        = false,
    };
}
