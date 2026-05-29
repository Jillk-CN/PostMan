using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// VHS 滤镜 Volume 参数控制器。
/// 负责将 VHSSettings 中的 Bloom / ChromaticAberration / Vignette 参数
/// 同步到场景 URP Volume Profile。
/// 与 VHSFilterEffect 完全独立，通过 VHSSettings 数据结构通信。
/// </summary>
public sealed class VHSVolumeController : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector 配置
    // ─────────────────────────────────────────────

    [Header("目标 Volume")]
    [Tooltip("挂载 SampleSceneProfile 的 Volume 对象")]
    [SerializeField] private Volume _targetVolume;

    [Header("目标相机")]
    [Tooltip("需要启用 Post Processing 的相机，留空则自动查找 Main Camera")]
    [SerializeField] private Camera _targetCamera;

    // ─────────────────────────────────────────────
    // 缓存引用
    // ─────────────────────────────────────────────

    // 缓存 Volume Component 引用，避免每帧 TryGet
    private Bloom _bloom;
    private ChromaticAberration _ca;
    private Vignette _vignette;

    // ─────────────────────────────────────────────
    // Unity 生命周期
    // ─────────────────────────────────────────────

    private void Awake()
    {
        if (_targetVolume == null || _targetVolume.profile == null)
        {
            Debug.LogError("[VHSVolumeController] 未指定 Volume 或 Profile 为空。");
            return;
        }

        // 创建运行时副本，避免修改持久化 Asset
        _targetVolume.profile = Instantiate(_targetVolume.profile);

        // 缓存各 Volume Component 引用
        _targetVolume.profile.TryGet(out _bloom);
        _targetVolume.profile.TryGet(out _ca);
        _targetVolume.profile.TryGet(out _vignette);

        // 如果 Profile 中没有 ChromaticAberration，动态添加
        if (_ca == null)
            _ca = _targetVolume.profile.Add<ChromaticAberration>(overrides: false);

        // 确保目标相机启用了 Post Processing，否则 Volume 效果（Bloom/CA/Vignette）不会渲染
        var cam = _targetCamera != null ? _targetCamera : Camera.main;
        if (cam != null)
        {
            var camData = cam.GetUniversalAdditionalCameraData();
            if (!camData.renderPostProcessing)
            {
                camData.renderPostProcessing = true;
                Debug.Log("[VHSVolumeController] 已自动启用相机 Post Processing。");
            }
        }
        else
        {
            Debug.LogWarning("[VHSVolumeController] 未找到目标相机，Post Processing 可能未启用。");
        }
    }

    // ─────────────────────────────────────────────
    // 公开接口
    // ─────────────────────────────────────────────

    /// <summary>
    /// 将 VHSSettings 中的 Volume 相关参数应用到 URP Volume。
    /// 由 VHSFilterController 在参数变化时调用。
    /// </summary>
    /// <param name="settings">当前参数快照。</param>
    public void Apply(in VHSSettings settings)
    {
        ApplyBloom(settings);
        ApplyCA(settings);
        ApplyVignette(settings);
    }

    // ─────────────────────────────────────────────
    // 私有：各效果参数同步
    // ─────────────────────────────────────────────

    /// <summary>同步 Bloom 参数到 Volume。</summary>
    private void ApplyBloom(in VHSSettings s)
    {
        if (_bloom == null) return;

        _bloom.active = s.bloomEnabled;
        // 禁用时将 overrideState 设为 false，确保 Volume 系统忽略这些参数
        _bloom.intensity.overrideState = s.bloomEnabled;
        _bloom.threshold.overrideState = s.bloomEnabled;
        _bloom.scatter.overrideState   = s.bloomEnabled;
        if (!s.bloomEnabled) return;

        _bloom.intensity.value = s.bloomIntensity;
        _bloom.threshold.value = s.bloomThreshold;
        _bloom.scatter.value   = s.bloomRadius; // scatter 即扩散半径
    }

    /// <summary>同步 Chromatic Aberration 参数到 Volume。</summary>
    private void ApplyCA(in VHSSettings s)
    {
        if (_ca == null) return;

        _ca.active = s.chromaticEnabled;
        // 禁用时将 overrideState 设为 false，确保 Volume 系统忽略该参数
        _ca.intensity.overrideState = s.chromaticEnabled;
        if (!s.chromaticEnabled) return;

        _ca.intensity.value = s.chromaticStrength;
    }

    /// <summary>同步 Vignette 参数到 Volume。</summary>
    private void ApplyVignette(in VHSSettings s)
    {
        if (_vignette == null) return;

        _vignette.active = s.vignetteEnabled;
        // 禁用时将 overrideState 设为 false，确保 Volume 系统忽略这些参数
        _vignette.intensity.overrideState   = s.vignetteEnabled;
        _vignette.smoothness.overrideState  = s.vignetteEnabled;
        if (!s.vignetteEnabled) return;

        _vignette.intensity.value  = s.vignetteIntensity;
        _vignette.smoothness.value = s.vignetteStrength;
    }
}
