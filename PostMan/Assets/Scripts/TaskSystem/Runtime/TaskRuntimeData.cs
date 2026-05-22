using System;

/// <summary>
/// 任务运行时状态快照（不可变类）。
/// 每次状态变更都返回新实例，原实例保持不变，避免隐式副作用。
/// </summary>
public sealed class TaskRuntimeData
{
    // ─────────────────────────────────────────────
    // 只读属性
    // ─────────────────────────────────────────────

    /// <summary>关联的任务定义资产。</summary>
    public TaskSO Definition { get; }

    /// <summary>当前进度值。</summary>
    public int CurrentProgress { get; }

    /// <summary>当前任务状态。</summary>
    public TaskStatus Status { get; }

    /// <summary>任务开始时的游戏时间（Time.time）。</summary>
    public float StartTime { get; }

    /// <summary>进度比例（0~1），用于 UI 进度条绑定。</summary>
    public float ProgressRatio =>
        Definition.totalProgress > 0
            ? (float)CurrentProgress / Definition.totalProgress
            : 0f;

    // ─────────────────────────────────────────────
    // 构造函数
    // ─────────────────────────────────────────────

    /// <summary>
    /// 创建一个新的任务运行时状态快照。
    /// </summary>
    /// <param name="definition">任务定义资产。</param>
    /// <param name="currentProgress">当前进度。</param>
    /// <param name="status">任务状态。</param>
    /// <param name="startTime">任务开始时间。</param>
    public TaskRuntimeData(TaskSO definition, int currentProgress, TaskStatus status, float startTime)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        CurrentProgress = currentProgress;
        Status = status;
        StartTime = startTime;
    }

    // ─────────────────────────────────────────────
    // 不可变更新方法（返回新实例）
    // ─────────────────────────────────────────────

    /// <summary>
    /// 返回一个更新了进度的新快照，原实例不变。
    /// </summary>
    /// <param name="newProgress">新的进度值。</param>
    public TaskRuntimeData WithProgress(int newProgress) =>
        new TaskRuntimeData(Definition, newProgress, Status, StartTime);

    /// <summary>
    /// 返回一个更新了状态的新快照，原实例不变。
    /// </summary>
    /// <param name="newStatus">新的任务状态。</param>
    public TaskRuntimeData WithStatus(TaskStatus newStatus) =>
        new TaskRuntimeData(Definition, CurrentProgress, newStatus, StartTime);
}
