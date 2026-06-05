using UnityEngine;
using PostMan.Player;

/// <summary>
/// 交互式任务推进器。
/// 玩家按 E 键交互时推进关联任务进度，继承自 BaseTaskAdvancer 并实现 IInteractable。
/// 配合 ShowInteractPrompt 显示交互提示，配合 LimitInteract 实现一次性交互。
/// </summary>
public class InteractableTaskAdvancer : BaseTaskAdvancer, IInteractable
{
    // ─────────────────────────────────────────────
    // 交互设置
    // ─────────────────────────────────────────────

    [Header("交互设置")]

    /// <summary>是否允许与此物体交互。</summary>
    [Tooltip("是否允许与此物体交互")]
    [SerializeField] private bool _canInteract = true;

    /// <summary>交互优先级，数值越大越先执行。</summary>
    [Tooltip("交互优先级，数值越大越先执行")]
    [SerializeField] private int _priority = 0;

    // ─────────────────────────────────────────────
    // IInteractable 实现
    // ─────────────────────────────────────────────

    /// <inheritdoc/>
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    /// <inheritdoc/>
    public int Priority { get => _priority; set => _priority = value; }

    /// <summary>玩家按 E 键时由 PlayerInteractor 调用，尝试推进任务进度。</summary>
    public void InteractWith(PlayerInteractor player) => TryAdvance();

    // ─────────────────────────────────────────────
    // BaseTaskAdvancer 实现
    // ─────────────────────────────────────────────

    /// <summary>始终满足推进条件，由玩家交互时机决定何时推进。</summary>
    protected override bool ShouldAdvance() => true;
}
