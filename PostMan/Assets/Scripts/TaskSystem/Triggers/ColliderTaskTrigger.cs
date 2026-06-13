using UnityEngine;

/// <summary>
/// 碰撞体任务触发器。
/// 当其他碰撞体进入此物体的触发区域时，启动关联任务。
/// 可选：触发成功后销毁指定物体。
/// </summary>
public class ColliderTaskTrigger : BaseTaskTrigger
{
    // ─────────────────────────────────────────────
    // 销毁配置
    // ─────────────────────────────────────────────

    [Header("触发后销毁")]

    /// <summary>触发成功后是否销毁目标物体。</summary>
    [Tooltip("触发成功后是否销毁目标物体")]
    [SerializeField] private bool destroyOnTrigger;

    /// <summary>触发成功后需要销毁的物体（destroyOnTrigger 为 true 时生效）。</summary>
    [Tooltip("触发成功后需要销毁的物体")]
    [SerializeField] private GameObject objectToDestroy;

    // ─────────────────────────────────────────────
    // 触发条件实现
    // ─────────────────────────────────────────────

    /// <summary>碰撞触发器始终满足触发条件，由 OnTriggerEnter 控制时机。</summary>
    protected override bool ShouldTrigger() => true;

    // ─────────────────────────────────────────────
    // Unity 物理回调
    // ─────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        TryTrigger(); // 尝试启动任务

        Debug.Log($"[ColliderTaskTrigger] 碰撞触发器被 {other.name} 触发，尝试启动任务 {targetTask?.taskLocalizationKey}。");

        // 触发成功后销毁指定物体
        if (destroyOnTrigger && objectToDestroy != null)
            Destroy(objectToDestroy);
    }
}
