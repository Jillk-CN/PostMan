using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using PostMan.Common;
using PostMan.InputManagement;
using PostMan.Player;

/// <summary>
/// 全局场景管理器，基于 Addressables 实现异步场景加载与卸载。
/// 使用 MonoSingleton 保证全局唯一，通过 DontDestroyOnLoad 跨场景存活。
/// </summary>
public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    // ─────────────────────────────────────────────
    // 调试设置
    // ─────────────────────────────────────────────

    [Header("调试设置")]

    /// <summary>是否输出详细日志。</summary>
    [Tooltip("开启后在 Console 输出场景加载/卸载的详细日志")]
    [SerializeField] private bool verboseLog = true;

    // ─────────────────────────────────────────────
    // 内部状态
    // ─────────────────────────────────────────────

    /// <summary>互斥锁：防止并发触发多次场景切换。</summary>
    private bool _isSwitching;

    /// <summary>已加载场景的句柄缓存，key 为 Addressable 场景地址。</summary>
    private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _loadedScenes
        = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

    // ─────────────────────────────────────────────
    // 单例初始化
    // ─────────────────────────────────────────────

    /// <summary>初始化单例，确保跨场景不销毁。</summary>
    protected override void Init()
    {
        //DontDestroyOnLoad(gameObject);
        Log("GameSceneManager 初始化完成，已标记 DontDestroyOnLoad。");
    }

    // ─────────────────────────────────────────────
    // 公开接口
    // ─────────────────────────────────────────────

    /// <summary>
    /// 切换场景：并行加载目标场景，再串行卸载旧场景。
    /// 若正在切换中则忽略本次调用。
    /// </summary>
    /// <param name="scenesToLoad">需要加载的场景地址列表（Addressable key）。</param>
    /// <param name="scenesToUnload">需要卸载的场景地址列表（Addressable key）。</param>
    /// <param name="spawnPoint">加载完成后玩家的传送坐标；为 null 则不传送（默认行为）。</param>
    public async void SwitchScenes(List<string> scenesToLoad, List<string> scenesToUnload, Vector3? spawnPoint = null)
    {
        // 互斥锁：防止并发切换
        if (_isSwitching)
        {
            Log("SwitchScenes 被忽略：上一次场景切换尚未完成。");
            return;
        }

        _isSwitching = true;

        try
        {
            // ── 阶段 1：并行加载所有目标场景 ──
            if (scenesToLoad != null && scenesToLoad.Count > 0)
            {
                Log($"开始并行加载 {scenesToLoad.Count} 个场景...");
                var loadTasks = new List<Task>();

                foreach (string address in scenesToLoad)
                {
                    // 已加载则跳过
                    if (_loadedScenes.ContainsKey(address))
                    {
                        Log($"场景 [{address}] 已在缓存中，跳过加载。");
                        continue;
                    }

                    loadTasks.Add(LoadSceneAsync(address));
                }

                await Task.WhenAll(loadTasks);
                Log("所有目标场景加载完成。");
            }

            // ── 阶段 1.5：重置输入源 + 传送玩家到出生点 ──
            // 所有目标场景已加载完毕，在卸载旧场景前将输入源归位，防止旧会话状态污染
            GameInputManager.Instance?.ResetToGameDefaults();

            if (spawnPoint.HasValue)
                TeleportPlayer(spawnPoint.Value);

            // ── 阶段 2：串行卸载旧场景 ──
            if (scenesToUnload != null && scenesToUnload.Count > 0)
            {
                Log($"开始串行卸载 {scenesToUnload.Count} 个场景...");

                foreach (string address in scenesToUnload)
                {
                    await UnloadSceneAsync(address);
                }

                Log("所有旧场景卸载完成。");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameSceneManager] 场景切换时发生异常：{e}");
        }
        finally
        {
            // 无论成功或失败，都释放锁
            _isSwitching = false;
        }
    }

    // ─────────────────────────────────────────────
    // 内部方法
    // ─────────────────────────────────────────────

    /// <summary>通过 Addressables 异步加载单个场景，并缓存句柄。</summary>
    private async Task LoadSceneAsync(string address)
    {
        Log($"加载场景：{address}");

        AsyncOperationHandle<SceneInstance> handle =
            Addressables.LoadSceneAsync(address, LoadSceneMode.Additive);

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _loadedScenes[address] = handle; // 缓存句柄供后续卸载使用
            Log($"场景 [{address}] 加载成功。");
        }
        else
        {
            Debug.LogError($"[GameSceneManager] 场景 [{address}] 加载失败：{handle.OperationException}");
        }
    }

    /// <summary>通过 Addressables 或 SceneManager 异步卸载单个场景，并移除缓存。</summary>
    private async Task UnloadSceneAsync(string address)
    {
        // ── 路径 A：Addressables 句柄路径（已通过 GameSceneManager 加载）──
        if (_loadedScenes.TryGetValue(address, out AsyncOperationHandle<SceneInstance> handle))
        {
            // 缓存句柄可能因 Addressables 内部 ref-count 归零而失效，先做有效性检查
            if (!handle.IsValid())
            {
                Debug.LogWarning($"[GameSceneManager] 场景 [{address}] 的缓存句柄已失效，清理缓存并降级到 fallback。");
                _loadedScenes.Remove(address);
                // 降级到路径 B，尝试用 SceneManager 卸载
            }
            else
            {
                Log($"卸载场景（Addressables）：{address}");
                AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(handle);
                await unloadHandle.Task;

                // unloadHandle 本身也可能在极端情况下变为无效（例如 handle 在 await 期间被释放）
                if (!unloadHandle.IsValid())
                {
                    _loadedScenes.Remove(address);
                    Debug.LogWarning($"[GameSceneManager] 场景 [{address}] 的卸载句柄在 await 后失效，已清理缓存。");
                    return;
                }

                if (unloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    _loadedScenes.Remove(address);
                    Log($"场景 [{address}] 卸载成功。");
                }
                else
                {
                    Debug.LogError($"[GameSceneManager] 场景 [{address}] 卸载失败：{unloadHandle.OperationException}");
                }
                return;
            }
        }

        // ── 路径 B：Fallback — 通过 SceneManager 按名称卸载（初始场景等）──
        string sceneName = Path.GetFileNameWithoutExtension(address);
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.LogWarning($"[GameSceneManager] 场景 [{address}] 未加载，跳过卸载。");
            return;
        }

        Log($"卸载场景（SceneManager fallback）：{sceneName}");
        await ToTask(SceneManager.UnloadSceneAsync(sceneName));
        Log($"场景 [{sceneName}] 卸载成功（fallback）。");
    }

    /// <summary>将 AsyncOperation 包装为 Task，供 await 使用。</summary>
    private static Task ToTask(AsyncOperation op)
    {
        var tcs = new TaskCompletionSource<bool>();
        op.completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }

    /// <summary>
    /// 将玩家传送到指定坐标。
    /// 传送前关闭 CharacterController 以避免内部状态不同步，传送后重新开启。
    /// </summary>
    /// <param name="position">目标世界坐标。</param>
    private void TeleportPlayer(Vector3 position)
    {
        var player = PlayerInstance.Instance;
        if (player == null)
        {
            Debug.LogWarning("[GameSceneManager] 传送玩家失败：PlayerInstance 不存在。");
            return;
        }

        // 必须先禁用 CharacterController，否则直接赋值 transform.position
        // 会在下一帧被 CC 内部状态推回原位
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = position;
        if (cc != null) cc.enabled = true;

        Log($"玩家已传送至 {position}。");
    }

    /// <summary>受 verboseLog 控制的日志输出。</summary>
    private void Log(string message)
    {
        if (verboseLog)
            Debug.Log($"[GameSceneManager] {message}");
    }
}
