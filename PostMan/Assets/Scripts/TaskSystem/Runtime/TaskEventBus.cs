using System;

/// <summary>
/// 任务事件总线（静态类）。
/// 作为所有任务事件的中央分发枢纽，彻底解耦 TaskManager 与 UI 及其他观察者。
/// 订阅者在 OnEnable 中注册，在 OnDisable 中注销，防止内存泄漏。
/// </summary>
public static class TaskEventBus
{
    // ─────────────────────────────────────────────
    // 公开事件（供外部订阅）
    // ─────────────────────────────────────────────

    /// <summary>任务被成功启动时触发。</summary>
    public static event Action<TaskRuntimeData> OnTaskStarted;

    /// <summary>任务进度推进时触发（未完成）。</summary>
    public static event Action<TaskRuntimeData> OnTaskAdvanced;

    /// <summary>任务进度达到总进度，成功完成时触发。</summary>
    public static event Action<TaskRuntimeData> OnTaskCompleted;

    /// <summary>任务被标记为失败时触发。</summary>
    public static event Action<TaskRuntimeData> OnTaskFailed;

    // ─────────────────────────────────────────────
    // 内部发布方法（仅由 TaskManager 调用）
    // ─────────────────────────────────────────────

    /// <summary>发布任务启动事件。</summary>
    internal static void PublishStarted(TaskRuntimeData data) =>
        OnTaskStarted?.Invoke(data);

    /// <summary>发布任务推进事件。</summary>
    internal static void PublishAdvanced(TaskRuntimeData data) =>
        OnTaskAdvanced?.Invoke(data);

    /// <summary>发布任务完成事件。</summary>
    internal static void PublishCompleted(TaskRuntimeData data) =>
        OnTaskCompleted?.Invoke(data);

    /// <summary>发布任务失败事件。</summary>
    internal static void PublishFailed(TaskRuntimeData data) =>
        OnTaskFailed?.Invoke(data);
}
