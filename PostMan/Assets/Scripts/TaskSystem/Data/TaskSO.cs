using UnityEngine;

/// <summary>
/// 任务数据定义资产（ScriptableObject）。
/// 存储任务的静态配置信息，在编辑器中通过右键菜单创建。
/// 运行时状态由 TaskManager 中的 TaskRuntimeData 持有，此类不存储进度。
/// </summary>
[CreateAssetMenu(menuName = "TaskSystem/Task", fileName = "NewTask")]
public class TaskSO : ScriptableObject
{
    // ─────────────────────────────────────────────
    // 任务标识
    // ─────────────────────────────────────────────

    [Header("任务标识")]

    /// <summary>任务唯一序号，用作字典键，同一项目中不可重复。</summary>
    [Tooltip("任务唯一序号，用作字典键，同一项目中不可重复")]
    public int taskIndex;

    /// <summary>任务名称，显示在 UI 标题处。</summary>
    [Tooltip("任务名称，显示在 UI 标题处")]
    public string taskName;

    // ─────────────────────────────────────────────
    // 任务描述
    // ─────────────────────────────────────────────

    [Header("任务描述")]

    /// <summary>任务内容说明，显示在 UI 描述区域。</summary>
    [Tooltip("任务内容说明，显示在 UI 描述区域")]
    [TextArea(2, 5)]
    public string taskDescription;

    // ─────────────────────────────────────────────
    // 任务进度
    // ─────────────────────────────────────────────

    [Header("任务进度")]

    /// <summary>完成任务所需的总进度值，至少为 1。</summary>
    [Tooltip("完成任务所需的总进度值，至少为 1")]
    [Min(1)]
    public int totalProgress = 1;
}
