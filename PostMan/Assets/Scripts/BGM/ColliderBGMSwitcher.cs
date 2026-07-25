using PostMan.AudioSystem;
using UnityEngine;

/// <summary>
/// 通过物理触发器（OnTriggerEnter）检测玩家进入指定区域，并自动切换 BGM。
/// 将此组件挂载到带有 Is Trigger Collider 的 GameObject，在 Inspector 中配置触发条件与新 BGM 片段。
/// 支持 Tag 过滤与单次触发保护，播放参数与 TaskBGMSwitcher 保持一致。
/// </summary>
public class ColliderBGMSwitcher : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 触发配置
    // ─────────────────────────────────────────────

    [Header("触发配置")]

    /// <summary>允许触发 BGM 切换的 Tag，空字符串表示接受任意碰撞体。</summary>
    [Tooltip("进入触发器的物体必须匹配此 Tag 才会切换 BGM；留空则接受任意碰撞体")]
    [SerializeField] private string triggerTag = "Player";

    /// <summary>是否只触发一次；勾选后重复进入区域不再切换 BGM。</summary>
    [Tooltip("勾选后只在首次进入时切换 BGM，之后重新进入不再触发")]
    [SerializeField] private bool triggerOnce = true;

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

    /// <summary>进入区域后要播放的新 BGM 片段，必须赋值。</summary>
    [Tooltip("进入触发区域后播放的新 BGM 片段，必须赋值")]
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

    /// <summary>单次触发保护标志；首次触发后置为 true。</summary>
    private bool _triggered = false;

    /// <summary>进入触发区域前正在播放的 BGM 片段快照，退出时用于还原。</summary>
    private AudioClip _previousClip;

    /// <summary>进入触发区域前 BGM 音轨的音量快照（固定为 1f，与 Play 默认值一致）。</summary>
    private float _previousVolume = 1f;

    // ─────────────────────────────────────────────
    // 物理触发
    // ─────────────────────────────────────────────

    /// <summary>
    /// 当碰撞体进入触发器时调用。
    /// 依次检查 newClip 非空、Tag 匹配、单次触发保护，通过后切换 BGM。
    /// </summary>
    /// <param name="other">进入触发器的碰撞体。</param>
    private void OnTriggerEnter(Collider other)
    {
        if (newClip == null)
        {
            Debug.LogError($"[ColliderBGMSwitcher] {name}：newClip 未赋值，无法切换 BGM。", this);
            return;
        }

        // Tag 过滤：triggerTag 非空时才比对
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        // 单次触发保护
        if (triggerOnce && _triggered) return;

        _triggered = true;

        // 快照当前 BGM，供退出时还原
        _previousClip   = AudioManager.Instance.GetClip(AudioTrackId.BGM);
        _previousVolume = 1f;

        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
        AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume);
    }

    /// <summary>
    /// 当碰撞体离开触发器时调用。
    /// 停止当前 BGM 并还原进入前的曲目；若进入前无 BGM 则只停止。
    /// </summary>
    /// <param name="other">离开触发器的碰撞体。</param>
    private void OnTriggerExit(Collider other)
    {
        // Tag 过滤：triggerTag 非空时才比对
        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        // 从未成功进入过，无需还原
        if (!_triggered) return;

        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);

        // 还原进入前的曲目（若进入前无 BGM 则只 Stop）
        if (_previousClip != null)
            AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, _previousVolume);
    }
}
