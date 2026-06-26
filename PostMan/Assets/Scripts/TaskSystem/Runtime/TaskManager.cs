using System.Collections.Generic;
using PostMan.Player;
using UnityEngine;

/// <summary>
/// 任务管理器（单例）。
/// 负责持有所有任务的运行时状态，并通过 TaskEventBus 广播状态变更。
/// 挂载到场景中的空物体上，自动跨场景保留（DontDestroyOnLoad）。
/// </summary>
public class TaskManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 单例
    // ─────────────────────────────────────────────

    /// <summary>全局单例访问点。</summary>
    public static TaskManager Instance { get; private set; }

    // ─────────────────────────────────────────────
    // 私有状态
    // ─────────────────────────────────────────────

    // 以 taskIndex 为键存储所有运行时任务状态
    private readonly Dictionary<int, TaskRuntimeData> _activeTasks =
        new Dictionary<int, TaskRuntimeData>();

    // ─────────────────────────────────────────────
    // Unity 生命周期
    // ─────────────────────────────────────────────

    private void Awake()
    {
        // 保证全局唯一，若已存在则销毁重复实例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 跨场景保留
    }

    // ─────────────────────────────────────────────
    // 公开接口（供外部代码调用）
    // ─────────────────────────────────────────────

    /// <summary>
    /// 启动一个任务。若任务已激活或已完成则忽略。
    /// </summary>
    /// <param name="task">要启动的任务资产。</param>
    /// <returns>启动成功返回 true，否则返回 false。</returns>
    public bool StartTask(TaskSO task)
    {
        if (task == null)
        {
            Debug.LogWarning("[TaskManager] StartTask: task 为 null，已忽略。");
            return false;
        }

        // 已存在且非 Inactive 状态则跳过
        if (_activeTasks.TryGetValue(task.taskIndex, out TaskRuntimeData existing) &&
            existing.Status != TaskStatus.Inactive)
        {
            return false;
        }

        // 创建不可变初始状态快照
        var data = new TaskRuntimeData(task, 0, TaskStatus.Active, Time.time);
        _activeTasks[task.taskIndex] = data;

        TaskEventBus.PublishStarted(data); // 广播启动事件
        return true;
    }

    /// <summary>
    /// 推进指定任务的进度。若任务未激活则忽略。
    /// 进度达到 totalProgress 时自动完成任务。
    /// </summary>
    /// <param name="taskIndex">任务序号。</param>
    /// <param name="amount">推进量，默认为 1。</param>
    /// <returns>推进成功返回 true，否则返回 false。</returns>
    public bool AdvanceTask(int taskIndex, int amount = 1)
    {
        if (!_activeTasks.TryGetValue(taskIndex, out TaskRuntimeData current))
        {
            Debug.LogWarning($"[TaskManager] AdvanceTask: 任务 {taskIndex} 不存在。");
            return false;
        }

        if (current.Status != TaskStatus.Active)
            return false;

        // 计算新进度，不超过总进度上限
        int newProgress = Mathf.Min(current.CurrentProgress + amount,
                                    current.Definition.totalProgress);

        if (newProgress >= current.Definition.totalProgress)
        {
            // 进度满足，标记为完成
            var completed = current.WithProgress(newProgress).WithStatus(TaskStatus.Completed);
            _activeTasks[taskIndex] = completed;
            TaskEventBus.PublishCompleted(completed); // 广播完成事件
        }
        else
        {
            // 进度推进但未完成
            var advanced = current.WithProgress(newProgress);
            _activeTasks[taskIndex] = advanced;
            TaskEventBus.PublishAdvanced(advanced); // 广播推进事件
        }

        return true;
    }

    /// <summary>
    /// 将指定任务标记为失败。若任务未激活则忽略。
    /// </summary>
    /// <param name="taskIndex">任务序号。</param>
    /// <returns>操作成功返回 true，否则返回 false。</returns>
    public bool FailTask(int taskIndex)
    {
        if (!_activeTasks.TryGetValue(taskIndex, out TaskRuntimeData current))
            return false;

        if (current.Status != TaskStatus.Active)
            return false;

        var failed = current.WithStatus(TaskStatus.Failed);
        _activeTasks[taskIndex] = failed;
        TaskEventBus.PublishFailed(failed); // 广播失败事件
        return true;
    }

    /// <summary>
    /// 获取指定任务的运行时状态快照。若不存在返回 null。
    /// </summary>
    /// <param name="taskIndex">任务序号。</param>
    public TaskRuntimeData GetTask(int taskIndex) =>
        _activeTasks.TryGetValue(taskIndex, out TaskRuntimeData data) ? data : null;

    /// <summary>判断指定任务是否处于激活状态。</summary>
    public bool IsTaskActive(int taskIndex) =>
        _activeTasks.TryGetValue(taskIndex, out TaskRuntimeData data) &&
        data.Status == TaskStatus.Active;

    /// <summary>判断指定任务是否已完成。</summary>
    public bool IsTaskCompleted(int taskIndex) =>
        _activeTasks.TryGetValue(taskIndex, out TaskRuntimeData data) &&
        data.Status == TaskStatus.Completed;

    
}
