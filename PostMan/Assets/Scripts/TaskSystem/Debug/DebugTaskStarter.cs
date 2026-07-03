using UnityEngine;

/// <summary>
/// 调试用任务启动器。
/// 在场景 Awake 阶段自动启动 Inspector 中指定的任务，
/// 用于测试时跳过已验证的前置任务，直接进入目标任务。
/// 启用/禁用该组件不影响其他任何模块的正常运行。
/// </summary>
public class DebugTaskStarter : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Inspector 配置
    // ─────────────────────────────────────────────

    [Header("调试任务配置")]
    [Tooltip("游戏开始时需要自动启动的任务列表。留空或禁用组件则不产生任何效果。")]
    [SerializeField] private TaskSO[] _tasksToStart;

    // ─────────────────────────────────────────────
    // Unity 生命周期
    // ─────────────────────────────────────────────

    /// <summary>
    /// 依次启动所有指定任务。
    /// 组件未启用则直接返回，不执行任何操作。
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!enabled) return;

            if (TaskManager.Instance == null)
            {
                Debug.LogWarning("[DebugTaskStarter] TaskManager 实例不存在，跳过任务启动。");
                return;
            }

            foreach (TaskSO task in _tasksToStart)
            {
                if (task == null) continue;
                TaskManager.Instance.StartTask(task);
            }
        }
    }
}
