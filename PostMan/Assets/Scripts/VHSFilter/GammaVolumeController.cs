using PostMan.Common;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Gamma（曝光）Volume 参数控制器（MonoSingleton）。
/// 负责将 SettingsPanel 的 Gamma 滑条值实时映射到 URP Volume 的 ColorAdjustments.postExposure。
/// 挂载到 Persistent 场景，通过 Inspector 引用场景 Volume。
/// </summary>
public sealed class GammaVolumeController : MonoSingleton<GammaVolumeController>
{
    // ─────────────────────────────────────────────
    // Inspector 配置
    // ─────────────────────────────────────────────

    [Header("目标 Volume")]
    [Tooltip("挂载 SampleSceneProfile 的 Volume 对象")]
    [SerializeField] private Volume _targetVolume;

    // ─────────────────────────────────────────────
    // 缓存引用
    // ─────────────────────────────────────────────

    private ColorAdjustments _colorAdjustments;

    // ─────────────────────────────────────────────
    // PlayerPrefs 键名（与 SettingsPanel 保持一致）
    // ─────────────────────────────────────────────

    private const string KeyGamma = "Gamma";

    // ─────────────────────────────────────────────
    // MonoSingleton 初始化
    // ─────────────────────────────────────────────

    protected override void Init()
    {
        if (_targetVolume == null || _targetVolume.profile == null)
        {
            Debug.LogError("[GammaVolumeController] 未指定 Volume 或 Profile 为空。");
            return;
        }

        // 创建运行时副本，避免修改持久化 Asset
        _targetVolume.profile = Instantiate(_targetVolume.profile);

        // 尝试获取 ColorAdjustments；Profile 中若不存在则动态添加
        if (!_targetVolume.profile.TryGet(out _colorAdjustments))
            _colorAdjustments = _targetVolume.profile.Add<ColorAdjustments>(overrides: false);

        // 确保 postExposure 参数生效
        _colorAdjustments.postExposure.overrideState = true;

        // 从 PlayerPrefs 恢复上次保存的值
        float saved = PlayerPrefs.GetFloat(KeyGamma, 1f);
        Apply(saved);
    }

    // ─────────────────────────────────────────────
    // 公开 API
    // ─────────────────────────────────────────────

    /// <summary>
    /// 将 Slider 值映射到 URP postExposure 并应用。
    /// 映射规则：sliderValue 1.0 → 0 EV（无变化）；0.5 → -1 EV（暗）；2.0 → +2 EV（亮）。
    /// 公式：postExposure = (sliderValue - 1f) * 2f
    /// </summary>
    /// <param name="sliderValue">Slider 原始值，建议值域 0.5 ~ 2.0。</param>
    public void Apply(float sliderValue)
    {
        if (_colorAdjustments == null) return;
        _colorAdjustments.postExposure.value = (sliderValue - 1f) * 2f;
    }
}
