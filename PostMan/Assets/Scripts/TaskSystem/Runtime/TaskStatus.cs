/// <summary>
/// 任务状态枚举，描述任务在生命周期中的各个阶段。
/// </summary>
public enum TaskStatus
{
    /// <summary>任务尚未开始。</summary>
    Inactive,

    /// <summary>任务已激活，正在进行中。</summary>
    Active,

    /// <summary>任务已成功完成。</summary>
    Completed,

    /// <summary>任务已失败。</summary>
    Failed
}
