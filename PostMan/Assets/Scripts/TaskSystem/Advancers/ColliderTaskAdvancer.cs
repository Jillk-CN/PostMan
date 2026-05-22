using UnityEngine;

/// <summary>
/// 碰撞体任务推进器。
/// 当其他碰撞体进入此物体的触发区域时，推进关联任务的进度。
/// 可选：推进成功后销毁指定物体。
/// </summary>
public class ColliderTaskAdvancer : BaseTaskAdvancer
{
    // ─────────────────────────────────────────────
    // 销毁配置
    // ─────────────────────────────────────────────

    [Header("推进后销毁")]

    /// <summary>推进成功后是否销毁目标物体。</summary>
    [Tooltip("推进成功后是否销毁目标物体")]
    [SerializeField] private bool destroyOnAdvance;

    /// <summary>推进成功后需要销毁的物体（destroyOnAdvance 为 true 时生效）。</summary>
    [Tooltip("推进成功后需要销毁的物体")]
    [SerializeField] private GameObject objectToDestroy;

    // ─────────────────────────────────────────────
    // 推进条件实现
    // ─────────────────────────────────────────────

    /// <summary>碰撞推进器始终满足推进条件，由 OnTriggerEnter 控制时机。</summary>
    protected override bool ShouldAdvance() => true;

    // ─────────────────────────────────────────────
    // Unity 物理回调
    // ─────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        TryAdvance(); // 尝试推进任务

        Debug.Log($"[ColliderTaskAdvancer] 碰撞推进器被 {other.name} 触发，尝试推进任务 {targetTask?.taskName}。");

        // 推进后销毁指定物体
        if (destroyOnAdvance && objectToDestroy != null)
            Destroy(objectToDestroy);
    }
}
