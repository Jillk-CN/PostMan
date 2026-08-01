using System.Collections.Generic;
using UnityEngine;
using PostMan.UI;

/// <summary>
/// 碰撞触发式场景切换组件。
/// 当其他碰撞体进入触发区域时，调用 GameSceneManager 执行场景切换。
/// 可配置 Tag 过滤、单次触发保护，与 ColliderTaskTrigger 风格一致。
/// </summary>
public class ColliderSceneChange : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // 场景配置
    // ─────────────────────────────────────────────

    [Header("场景配置")]

    /// <summary>需要加载的场景地址列表（Addressable key）。</summary>
    [Tooltip("触发后需要加载的场景（Addressable 地址）")]
    [SerializeField] private List<string> _scenesToLoad = new List<string>();

    /// <summary>需要卸载的场景地址列表（Addressable key）。</summary>
    [Tooltip("触发后需要卸载的场景（Addressable 地址）")]
    [SerializeField] private List<string> _scenesToUnload = new List<string>();

    /// <summary>是否启用出生点传送。</summary>
    [Tooltip("是否启用出生点传送")]
    [SerializeField] private bool _useSpawnPoint = false;

    /// <summary>场景切换后玩家的出生坐标。</summary>
    [Tooltip("场景切换后玩家的出生坐标（仅 Use Spawn Point 开启时生效）")]
    [SerializeField] private Vector3 _spawnPoint;

    // ─────────────────────────────────────────────
    // 触发条件
    // ─────────────────────────────────────────────

    [Header("触发条件")]

    /// <summary>进入触发区域的碰撞体必须具有此 Tag 才能触发（留空则不过滤）。</summary>
    [Tooltip("只有具有此 Tag 的碰撞体才能触发场景切换；留空则任意碰撞体均可触发")]
    [SerializeField] private string _requiredTag = string.Empty;

    // ─────────────────────────────────────────────
    // 触发行为
    // ─────────────────────────────────────────────

    [Header("触发行为")]

    /// <summary>是否只触发一次（触发后忽略后续碰撞）。</summary>
    [Tooltip("开启后首次触发后不再响应后续碰撞")]
    [SerializeField] private bool _triggerOnce = true;

    /// <summary>是否已经触发过（用于单次触发保护）。</summary>
    private bool _hasTriggered;

    // ─────────────────────────────────────────────
    // Unity 物理回调
    // ─────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        // 单次触发保护
        if (_triggerOnce && _hasTriggered)
            return;

        // Tag 过滤：留空则不过滤
        if (!string.IsNullOrEmpty(_requiredTag) && !other.CompareTag(_requiredTag))
            return;

        _hasTriggered = true;

        Debug.Log($"[ColliderSceneChange] 被 {other.name} 触发，开始切换场景。");
        BlackScreen.Instance?.BlackIn("", 1f, () =>
        {
            GameSceneManager.Instance?.SwitchScenes(
                _scenesToLoad,
                _scenesToUnload,
                _useSpawnPoint ? _spawnPoint : (Vector3?)null
            );
            BlackScreen.Instance?.BlackOut("", 1f);
        });
    }
}
