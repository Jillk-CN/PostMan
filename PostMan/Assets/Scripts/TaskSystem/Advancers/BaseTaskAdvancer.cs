using UnityEngine;

/// <summary>
/// 任务推进器抽象基类。
/// 封装调用 TaskManager.AdvanceTask 的通用逻辑，包含单次推进保护。
/// 子类只需实现 ShouldAdvance() 定义推进条件，无需关心管理器调用细节。
/// </summary>
public abstract class BaseTaskAdvancer : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 任务配置
    // ─────────────────────────────────────────────

    [Header("任务配置")]

    /// <summary>此推进器将要推进的任务资产。</summary>
    [Tooltip("此推进器将要推进的任务资产")]
    [SerializeField] protected TaskSO targetTask;

    // ─────────────────────────────────────────────
    // 推进设置
    // ─────────────────────────────────────────────

    [Header("推进设置")]

    /// <summary>每次激活时增加的进度量。</summary>
    [Tooltip("每次激活时增加的进度量")]
    [SerializeField] protected int advanceAmount = 1;

    /// <summary>若为 true，此推进器在整个会话中只能推进一次。</summary>
    [Tooltip("若为 true，此推进器在整个会话中只能推进一次")]
    [SerializeField] protected bool advanceOnce = false;

    // ─────────────────────────────────────────────
    // 私有状态
    // ─────────────────────────────────────────────

    private bool _hasAdvanced; // 记录是否已推进过（配合 advanceOnce 使用）

    // ─────────────────────────────────────────────
    // 抽象接口（子类实现）
    // ─────────────────────────────────────────────

    /// <summary>
    /// 子类实现此方法，返回当前是否满足推进条件。
    /// </summary>
    protected abstract bool ShouldAdvance();

    // ─────────────────────────────────────────────
    // 受保护方法（供子类调用）
    // ─────────────────────────────────────────────

    /// <summary>
    /// 尝试推进任务进度。包含单次推进保护和条件检查。
    /// 子类在合适时机调用此方法。
    /// </summary>
    protected void TryAdvance()
    {
        if (advanceOnce && _hasAdvanced) return; // 已推进过则跳过
        if (!ShouldAdvance()) return;            // 条件不满足则跳过

        if (TaskManager.Instance == null)
        {
            Debug.LogWarning("[BaseTaskAdvancer] 场景中未找到 TaskManager 实例。");
            return;
        }

        bool advanced = TaskManager.Instance.AdvanceTask(targetTask.taskIndex, advanceAmount);

        if (advanced && advanceOnce)
            _hasAdvanced = true; // 标记已推进
    }
}
