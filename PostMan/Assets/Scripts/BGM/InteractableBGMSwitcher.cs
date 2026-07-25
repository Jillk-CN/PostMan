using PostMan.AudioSystem;
using PostMan.Player;
using UnityEngine;

/// <summary>
/// 实现 IInteractable 接口，当玩家与该物体产生交互时自动切换 BGM。
/// 将此组件挂载到带有 Collider 的场景 GameObject，在 Inspector 中配置交互参数与新 BGM 片段。
/// 支持单次触发保护，以及任务完成时还原切换前 BGM 的功能（与 TaskBGMSwitcher 保持一致）。
/// </summary>
public class InteractableBGMSwitcher : MonoBehaviour, IInteractable
{
    // ─────────────────────────────────────────────
    // 交互配置
    // ─────────────────────────────────────────────

    [Header("交互配置")]

    /// <summary>是否允许玩家交互；false 时 PlayerInteractor 跳过此组件。</summary>
    [Tooltip("控制是否可被玩家交互；取消勾选后玩家无法触发此 BGM 切换")]
    public bool canInteract = true;

    /// <summary>交互优先级；同一物体挂多个 IInteractable 时，优先级大的先执行。</summary>
    [Tooltip("同一物体上有多个 IInteractable 组件时，优先级大的先被调用")]
    public int priority = 0;

    /// <summary>是否只触发一次；勾选后再次交互不再切换 BGM。</summary>
    [Tooltip("勾选后只在首次交互时切换 BGM，之后重复交互不再触发")]
    [SerializeField] private bool interactOnce = true;

    // IInteractable 属性实现
    public bool CanInteract { get => canInteract; set => canInteract = value; }
    public int  Priority    { get => priority;    set => priority    = value; }

    // ─────────────────────────────────────────────
    // Task 还原配置
    // ─────────────────────────────────────────────

    [Header("Task 还原配置")]

    /// <summary>启用后，当目标任务完成时自动停止当前 BGM 并还原交互前的曲目。</summary>
    [Tooltip("勾选后，目标任务完成时停止当前 BGM 并还原交互前的曲目；未勾选则不做任何处理")]
    [SerializeField] private bool restoreOnTaskComplete = false;

    /// <summary>触发还原的目标任务定义资产；restoreOnTaskComplete 启用时必须赋值。</summary>
    [Tooltip("目标任务完成时触发 BGM 还原，必须在启用 restoreOnTaskComplete 时赋值")]
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

    /// <summary>交互触发后要播放的新 BGM 片段，必须赋值。</summary>
    [Tooltip("玩家交互后播放的新 BGM 片段，必须赋值")]
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
    // 运行时状态
    // ─────────────────────────────────────────────

    /// <summary>单次触发保护标志兼还原入口守卫；首次交互成功后置为 true。</summary>
    private bool _triggered = false;

    /// <summary>交互前正在播放的 BGM 快照，任务完成时用于还原。</summary>
    private AudioClip _previousClip;

    // ─────────────────────────────────────────────
    // 生命周期
    // ─────────────────────────────────────────────

    private void OnEnable()  => TaskEventBus.OnTaskCompleted += HandleTaskCompleted;
    private void OnDisable() => TaskEventBus.OnTaskCompleted -= HandleTaskCompleted;

    // ─────────────────────────────────────────────
    // IInteractable 实现
    // ─────────────────────────────────────────────

    /// <summary>
    /// 玩家交互时调用。
    /// 依次检查 newClip 非空、单次触发保护，通过后快照当前 BGM 并切换到新曲目。
    /// </summary>
    /// <param name="player">触发交互的 PlayerInteractor。</param>
    public void InteractWith(PlayerInteractor player)
    {
        if (newClip == null)
        {
            Debug.LogError($"[InteractableBGMSwitcher] {name}：newClip 未赋值，无法切换 BGM。", this);
            return;
        }

        // 单次触发保护
        if (interactOnce && _triggered) return;

        _triggered    = true;
        _previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM);

        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
        AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume);
    }

    // ─────────────────────────────────────────────
    // 事件处理
    // ─────────────────────────────────────────────

    /// <summary>
    /// 接收 TaskEventBus.OnTaskCompleted 事件。
    /// 仅当 restoreOnTaskComplete 启用、BGM 已切换且任务 index 匹配时还原之前的 BGM。
    /// </summary>
    /// <param name="data">任务运行时快照。</param>
    private void HandleTaskCompleted(TaskRuntimeData data)
    {
        if (!restoreOnTaskComplete) return;

        // 切换从未发生，无需还原
        if (!_triggered) return;

        if (targetTask == null) return;

        // 仅响应目标任务
        if (data.Definition.taskIndex != targetTask.taskIndex) return;

        _triggered = false;

        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);

        // 还原交互前的曲目（若交互前无 BGM 则只 Stop）
        if (_previousClip != null)
            AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f);
    }
}
