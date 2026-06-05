using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;

/// <summary>
/// 交互式场景切换组件。
/// 玩家按 E 键交互时调用 GameSceneManager 执行场景切换，实现 IInteractable 接口。
/// 配合 ShowInteractPrompt 显示交互提示，配合 LimitInteract 实现一次性交互。
/// </summary>
public class InteractableSceneChange : MonoBehaviour, IInteractable
{
    // ─────────────────────────────────────────────
    // 场景配置
    // ─────────────────────────────────────────────

    [Header("场景配置")]

    /// <summary>需要加载的场景地址列表（Addressable key）。</summary>
    [Tooltip("交互后需要加载的场景（Addressable 地址）")]
    [SerializeField] private List<string> _scenesToLoad = new List<string>();

    /// <summary>需要卸载的场景地址列表（Addressable key）。</summary>
    [Tooltip("交互后需要卸载的场景（Addressable 地址）")]
    [SerializeField] private List<string> _scenesToUnload = new List<string>();

    /// <summary>是否启用出生点传送。</summary>
    [Tooltip("是否启用出生点传送")]
    [SerializeField] private bool _useSpawnPoint = false;

    /// <summary>场景切换后玩家的出生坐标。</summary>
    [Tooltip("场景切换后玩家的出生坐标（仅 Use Spawn Point 开启时生效）")]
    [SerializeField] private Vector3 _spawnPoint;

    // ─────────────────────────────────────────────
    // 交互设置
    // ─────────────────────────────────────────────

    [Header("交互设置")]

    /// <summary>是否允许与此物体交互。</summary>
    [Tooltip("是否允许玩家与此物体交互")]
    [SerializeField] private bool _canInteract = true;

    /// <summary>交互优先级，数值越大越先执行。</summary>
    [Tooltip("交互优先级，数值越大越先执行（多个 IInteractable 同时存在时生效）")]
    [SerializeField] private int _priority = 0;

    // ─────────────────────────────────────────────
    // 触发行为
    // ─────────────────────────────────────────────

    [Header("触发行为")]

    /// <summary>是否只交互一次（交互后禁止再次触发）。</summary>
    [Tooltip("开启后首次交互后不再响应后续交互请求")]
    [SerializeField] private bool _interactOnce = true;

    /// <summary>是否已经交互过（用于单次交互保护）。</summary>
    private bool _hasInteracted;

    // ─────────────────────────────────────────────
    // IInteractable 实现
    // ─────────────────────────────────────────────

    /// <inheritdoc/>
    public bool CanInteract { get => _canInteract; set => _canInteract = value; }

    /// <inheritdoc/>
    public int Priority { get => _priority; set => _priority = value; }

    /// <summary>玩家按 E 键时由 PlayerInteractor 调用，执行场景切换。</summary>
    /// <param name="player">触发交互的玩家。</param>
    public void InteractWith(PlayerInteractor player)
    {
        // 单次交互保护
        if (_interactOnce && _hasInteracted)
            return;

        _hasInteracted = true;

        Debug.Log($"[InteractableSceneChange] 玩家 {player.name} 触发交互，开始切换场景。");
        GameSceneManager.Instance?.SwitchScenes(
            _scenesToLoad,
            _scenesToUnload,
            _useSpawnPoint ? _spawnPoint : (Vector3?)null
        );
    }
}
