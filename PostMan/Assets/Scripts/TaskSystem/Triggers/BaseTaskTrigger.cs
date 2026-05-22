using UnityEngine;

/// <summary>
/// 任务触发器抽象基类。
/// 封装调用 TaskManager.StartTask 的通用逻辑，包含单次触发保护。
/// 子类只需实现 ShouldTrigger() 定义触发条件，无需关心管理器调用细节。
/// </summary>
public abstract class BaseTaskTrigger : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 任务配置
    // ─────────────────────────────────────────────

    [Header("任务配置")]

    /// <summary>此触发器将要启动的任务资产。</summary>
    [Tooltip("此触发器将要启动的任务资产")]
    [SerializeField] protected TaskSO targetTask;

    // ─────────────────────────────────────────────
    // 触发行为
    // ─────────────────────────────────────────────

    [Header("触发行为")]

    /// <summary>若为 true，此触发器在整个会话中只能触发一次。</summary>
    [Tooltip("若为 true，此触发器在整个会话中只能触发一次")]
    [SerializeField] protected bool triggerOnce = true;

    // ─────────────────────────────────────────────
    // 私有状态
    // ─────────────────────────────────────────────

    private bool _hasTriggered = false; // 记录是否已触发过（配合 triggerOnce 使用）

    // ─────────────────────────────────────────────
    // 抽象接口（子类实现）
    // ─────────────────────────────────────────────

    /// <summary>
    /// 子类实现此方法，返回当前是否满足触发条件。
    /// </summary>
    protected abstract bool ShouldTrigger();

    // ─────────────────────────────────────────────
    // 受保护方法（供子类调用）
    // ─────────────────────────────────────────────

    /// <summary>
    /// 尝试触发任务启动。包含单次触发保护和条件检查。
    /// 子类在合适时机调用此方法。
    /// </summary>
    protected void TryTrigger()
    {
        if (triggerOnce && _hasTriggered) return; // 已触发过则跳过
        if (!ShouldTrigger()) return;             // 条件不满足则跳过

        if (TaskManager.Instance == null)
        {
            Debug.LogWarning("[BaseTaskTrigger] 场景中未找到 TaskManager 实例。");
            return;
        }

        bool started = TaskManager.Instance.StartTask(targetTask);

        if (started && triggerOnce)
            _hasTriggered = true; // 标记已触发
    }
}
