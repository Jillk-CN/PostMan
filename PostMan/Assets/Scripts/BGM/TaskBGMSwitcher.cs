using PostMan.AudioSystem;
using UnityEngine;

/// <summary>
/// 监听指定任务的启动事件，并自动切换 BGM。
/// 将此组件挂载到场景中任意 GameObject，在 Inspector 中配置目标任务与新 BGM 片段。
/// 组件禁用后不再响应任务事件，可安全地在需要时关闭。
/// </summary>
public class TaskBGMSwitcher : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Task 触发配置
    // ─────────────────────────────────────────────

    [Header("Task 触发配置")]

    /// <summary>触发 BGM 切换的目标任务定义资产。</summary>
    [Tooltip("当此任务启动时切换 BGM，必须赋值")]
    [SerializeField] private TaskSO targetTask;

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

    /// <summary>任务启动后要播放的新 BGM 片段，必须赋值。</summary>
    [Tooltip("任务启动后播放的新 BGM 片段，必须赋值")]
    [SerializeField] private AudioClip newClip;

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
    // 生命周期
    // ─────────────────────────────────────────────

    private void OnEnable()  => TaskEventBus.OnTaskStarted += HandleTaskStarted;
    private void OnDisable() => TaskEventBus.OnTaskStarted -= HandleTaskStarted;

    // ─────────────────────────────────────────────
    // 事件处理
    // ─────────────────────────────────────────────

    /// <summary>
    /// 接收 TaskEventBus.OnTaskStarted 事件。
    /// 仅当启动的任务与 targetTask 的 taskIndex 匹配时执行 BGM 切换。
    /// </summary>
    /// <param name="data">任务运行时快照。</param>
    private void HandleTaskStarted(TaskRuntimeData data)
    {
        if (targetTask == null || newClip == null)
        {
            Debug.LogError($"[TaskBGMSwitcher] {name}：targetTask 或 newClip 未赋值，无法切换 BGM。", this);
            return;
        }

        // 仅响应目标任务
        if (data.Definition.taskIndex != targetTask.taskIndex) return;

        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
        AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume);
    }
}
