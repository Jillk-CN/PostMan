using System.Collections.Generic;
using System.Linq;
using PostMan.AudioSystem;
using UnityEngine;
using PostMan.Scene;

/// <summary>
/// 监听场景切换完成事件，并自动切换 BGM。
/// 将此组件挂载到场景中任意持久化 GameObject，在 Inspector 中配置目标场景地址与新 BGM 片段。
/// 当 GameSceneManager.OnSceneSwitchCompleted 触发且加载列表包含目标场景地址时执行 BGM 切换。
/// 同时支持通过 SceneInitializer 的 sceneOrder 触发播放另一 BGM 片段。
/// 组件禁用后不再响应场景事件，可安全地在需要时关闭。
/// </summary>
public class SceneBGMSwitcher : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 场景触发配置
    // ─────────────────────────────────────────────

    [Header("场景触发配置")]

    /// <summary>触发 BGM 切换的目标场景 Addressable 地址（与 GameSceneManager.SwitchScenes 中的 key 一致）。</summary>
    [Tooltip("目标场景的 Addressable key，与 GameSceneManager.SwitchScenes 中传入的地址一致，必须赋值")]
    [SerializeField] private string targetSceneAddress;

    /// <summary>是否只触发一次；勾选后同一场景重复加载不再切换 BGM。</summary>
    [Tooltip("勾选后只在首次加载目标场景时切换 BGM，之后重新加载不再触发")]
    [SerializeField] private bool triggerOnce = true;

    /// <summary>启用后，目标场景被卸载时自动停止当前 BGM 并还原加载前的曲目。</summary>
    [Tooltip("勾选后，目标场景卸载时停止当前 BGM 并还原加载前的曲目；未勾选则不做任何处理")]
    [SerializeField] private bool restoreOnUnload = false;

    // ─────────────────────────────────────────────
    // SceneOrder 触发配置
    // ─────────────────────────────────────────────

    [Header("SceneOrder 触发配置")]

    /// <summary>启用后，当 SceneInitializer 的 sceneOrder 到达目标值时播放 newClip2。</summary>
    [Tooltip("启用后才监听 SceneInitializer 的 sceneOrder 回调；禁用时此分支完全不生效")]
    [SerializeField] private bool enableSceneOrderTrigger = false;

    /// <summary>触发 newClip2 播放的目标 sceneOrder 值（与 SceneInitializer.SceneOrder 对应）。</summary>
    [Tooltip("目标 sceneOrder 值，与 SceneInitializer 内部计数一致（每次场景切换完成后 +1）")]
    [SerializeField] private int targetSceneOrder = 0;

    /// <summary>SceneOrder 分支是否只触发一次。</summary>
    [Tooltip("勾选后 sceneOrder 分支只在首次匹配时播放，之后重新进入相同 sceneOrder 不再触发")]
    [SerializeField] private bool triggerOnce2 = true;

    // ─────────────────────────────────────────────
    // Stop 参数
    // ─────────────────────────────────────────────

    [Header("Stop 参数")]

    /// <summary>停止当前 BGM 时是否淡出。</summary>
    [Tooltip("停止当前 BGM 时是否淡出")]
    [SerializeField] private bool stopFadeOut = true;

    /// <summary>淡出时长（秒）。</summary>
    [Tooltip("淡出时长（秒）")]
    [SerializeField] private float stopFadeOutDuration = 0.5f;

    // ─────────────────────────────────────────────
    // Play 参数
    // ─────────────────────────────────────────────

    [Header("Play 参数")]

    /// <summary>场景加载完成后要播放的新 BGM 片段，必须赋值（场景地址分支使用）。</summary>
    [Tooltip("目标场景加载完成后播放的新 BGM 片段，必须赋值")]
    [SerializeField] private AudioClip newClip;

    /// <summary>SceneOrder 分支触发时播放的 BGM 片段；仅在 enableSceneOrderTrigger 启用时需要赋值。</summary>
    [Tooltip("SceneOrder 分支触发时播放的新 BGM 片段；启用 SceneOrder 触发时必须赋值")]
    [SerializeField] private AudioClip newClip2;

    /// <summary>新 BGM 是否循环播放。</summary>
    [Tooltip("新 BGM 是否循环播放")]
    [SerializeField] private bool loop = true;

    /// <summary>播放新 BGM 时是否淡入。</summary>
    [Tooltip("播放新 BGM 时是否淡入")]
    [SerializeField] private bool playFadeIn = true;

    /// <summary>淡入时长（秒）。</summary>
    [Tooltip("淡入时长（秒）")]
    [SerializeField] private float playFadeInDuration = 1f;

    /// <summary>新 BGM 的局部音量（0~1）。</summary>
    [Tooltip("新 BGM 的局部音量（0~1），最终受 MixerGroup 调节")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    // ─────────────────────────────────────────────
    // 运行时状态
    // ─────────────────────────────────────────────

    /// <summary>单次触发保护标志（场景地址分支）；首次触发后置为 true。</summary>
    private bool _triggered = false;

    /// <summary>加载 targetSceneAddress 前正在播放的 BGM 快照，卸载时用于还原。</summary>
    private AudioClip _previousClip;

    /// <summary>单次触发保护标志（sceneOrder 分支）；首次触发后置为 true。</summary>
    private bool _triggered2 = false;

    // ─────────────────────────────────────────────
    // 生命周期
    // ─────────────────────────────────────────────

    private void OnEnable()
    {
        GameSceneManager.OnSceneSwitchCompleted += HandleSceneSwitchCompleted;
        SceneInitializer.Instance.Register(HandleSceneOrder);
    }

    private void OnDisable()
    {
        GameSceneManager.OnSceneSwitchCompleted -= HandleSceneSwitchCompleted;
    }

    // ─────────────────────────────────────────────
    // 事件处理
    // ─────────────────────────────────────────────

    /// <summary>
    /// 接收 GameSceneManager.OnSceneSwitchCompleted 事件。
    /// 仅当加载的场景列表包含 targetSceneAddress 时执行 BGM 切换。
    /// </summary>
    /// <param name="loadedScenes">本次场景切换中加载的场景地址只读列表。</param>
    private void HandleSceneSwitchCompleted(IReadOnlyList<string> loadedScenes)
    {
        if (string.IsNullOrEmpty(targetSceneAddress))
        {
            Debug.LogError($"[SceneBGMSwitcher] {name}：targetSceneAddress 未填写，无法切换 BGM。", this);
            return;
        }

        // 卸载还原分支：已切换过 BGM 且本次切换不含目标场景 → 目标场景已被卸载
        if (restoreOnUnload && _triggered && !loadedScenes.Contains(targetSceneAddress))
        {
            _triggered = false;
            AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
            if (_previousClip != null)
                AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f);
            return;
        }

        if (newClip == null)
        {
            //Debug.LogError($"[SceneBGMSwitcher] {name}：newClip 未赋值，无法切换 BGM。", this);
            return;
        }

        // 单次触发保护
        if (triggerOnce && _triggered) return;

        // 仅响应包含目标场景地址的切换
        if (!loadedScenes.Contains(targetSceneAddress)) return;

        // 快照当前 BGM，供卸载时还原
        _previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM);

        _triggered = true;

        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
        AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume);

        Debug.LogWarning($"[SceneBGMSwitcher] {name}：已切换 BGM 至 {newClip.name}，目标场景地址：{targetSceneAddress}，当前场景次序：{SceneInitializer.Instance.SceneOrder}", this);
    }

    /// <summary>
    /// 接收 SceneInitializer 的 sceneOrder 回调。
    /// 仅当 enableSceneOrderTrigger 启用且 sceneOrder 与 targetSceneOrder 匹配时播放 newClip2。
    /// </summary>
    /// <param name="sceneOrder">当前场景次序（每次场景切换完成后 +1）。</param>
    /// <param name="sceneName">场景名称（当前传入为空字符串）。</param>
    private void HandleSceneOrder(int sceneOrder, string sceneName)
    {
        if (!enableSceneOrderTrigger) return;

        if (newClip2 == null)
        {
            //Debug.LogError($"[SceneBGMSwitcher] {name}：newClip2 未赋值，无法执行 SceneOrder BGM 切换。", this);
            return;
        }

        // 单次触发保护
        if (triggerOnce2 && _triggered2) return;

        // 仅响应目标 sceneOrder
        if (sceneOrder != targetSceneOrder) return;

        _triggered2 = true;

        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
        AudioManager.Instance.Play(AudioTrackId.BGM, newClip2, loop, playFadeIn, playFadeInDuration, volume);
    }
}
