using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PostMan.Common;
using PostMan.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 交互物全局管理器，提供接口控制场景上交互物状态
/// </summary>
public class InteractableManager : MonoSingleton<InteractableManager>
{
    //场景上交互物的缓存
    private Dictionary<string , ItemState> _ObjectCache = new Dictionary<string, ItemState>();

    //记录已经加载过交互物缓存的场景
    private List<string> _Sceneloaded = new List<string>();

    /// <summary>
    /// 场景名 → 进入该场景时自动应用的状态清单（延迟队列）
    /// </summary>
    private Dictionary<string, InteractablesProcessListSO> _pendingProcessLists = new Dictionary<string, InteractablesProcessListSO>();

    //调试
    public bool isTestState = false;

    private void Awake()
    {
        // 如果已有实例且不是当前对象，则销毁自身（防止场景重复加载导致重复实例）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // 游戏启动时，当前场景已经加载完成，需要手动触发一次
        // 但如果场景加载在 Start 之前触发，可能已经执行过了
        //UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        //OnSceneLoaded(currentScene, LoadSceneMode.Single);
        
    }

    /// <summary>
    /// 场景切换后检测当前场景是否已经建立缓存
    /// 场景切换后重新绑定交互物GameObject
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //若当前场景未建立过缓存，则先建立缓存
        if(!_Sceneloaded.Contains(scene.name))
        {
            _Sceneloaded.Add(scene.name);

            BuildCache(scene);
        }
        else if(_Sceneloaded.Contains(scene.name))
        {
            ReBuildCache(scene);

            // 恢复该场景交互物的上次状态
            ApplyState();
        }

        // 应用该场景待处理的状态清单（在缓存构建/重建之后执行，确保缓存已存在）
        ApplyPendingProcessList(scene.name);
    }

    /// <summary>
    /// 建立当前场景上交互物的缓存
    /// </summary>
    private void BuildCache(UnityEngine.SceneManagement.Scene scene)
    {
        ObjectId[] objectIds = FindObjectsOfType<ObjectId>(true);

        foreach(var objectId in objectIds)
        {
            //检查是否为当前场景物体
            if(objectId.gameObject.scene != scene) continue;

            //检查是否已经添加
            if(_ObjectCache.ContainsKey(objectId.uniqueID)) continue;

            //把当前遍历到的交互物加载进缓存
            ItemState _objectState = new ItemState();

            _objectState.uniqueID = objectId.uniqueID;

            _objectState.gameObject = objectId.gameObject;

            IInteractable interactScript = GetInteractScript(objectId.gameObject);

            if(interactScript != null)
            {
                _objectState.isCanInteract = interactScript.CanInteract;
            }
            else if(interactScript == null)
            {
                Debug.LogWarning($"[交互物全局管理器] 对象 {objectId.name} 缺失交互组件IInteractable");
            }

            _objectState.isVisuable = objectId.transform.gameObject.activeSelf;

            _ObjectCache.Add(objectId.uniqueID , _objectState);
            
            if(isTestState)
            Debug.Log($"[交互物全局管理器] 成功把对象 {objectId.name} 载入缓存");
        }

        Debug.Log($"[交互物全局管理器] 场景 {scene.name} 交互物缓存构建完成，共 {objectIds.Count()} / {objectIds.Count()}个交互物");
        
        if(objectIds.Count() == 0)
        {
            Debug.LogWarning("[交互物全局管理器] 当前场景没有找到任何交互物");
        }
    }

    /// <summary>
    /// 切换场景后重新绑定交互物的GameObject
    /// 若缓存中找不到该交互物（例如跨场景新交互物），则重新创建缓存条目
    /// 若缓存中存在该交互物但gameObject已销毁，保留保存的状态仅更新gameObject引用
    /// </summary>
    private void ReBuildCache(UnityEngine.SceneManagement.Scene scene)
    {
        ObjectId[] objectIds = FindObjectsOfType<ObjectId>(true);

        foreach(var objectId in objectIds)
        {
            //检查是否为当前场景物体
            if(objectId.gameObject.scene != scene) continue;

            //根据当前遍历到的交互物的ObjecyId获取对应键值，再重新绑定GameObject
            ItemState _objectState;

            if(_ObjectCache.TryGetValue(objectId.uniqueID , out _objectState))
            {
                // 保留缓存中的isVisuable/isCanInteract状态，仅更新gameObject引用
                _objectState.gameObject = objectId.gameObject;

                if(isTestState)
                Debug.Log($"[交互物全局管理器] 成功把对象 {objectId.name} 重新绑定缓存");
            }
            else
            {
                // 缓存中没有该条目（新交互物），创建新条目
                ItemState newState = new ItemState();
                newState.uniqueID = objectId.uniqueID;
                newState.gameObject = objectId.gameObject;

                IInteractable interactScript = GetInteractScript(objectId.gameObject);
                if(interactScript != null)
                {
                    newState.isCanInteract = interactScript.CanInteract;
                }

                newState.isVisuable = objectId.transform.gameObject.activeSelf;

                _ObjectCache.Add(objectId.uniqueID, newState);

                Debug.Log($"[交互物全局管理器] 新建缓存条目: {objectId.name} (UniqueID: {objectId.uniqueID})");
            }
        }
    }

    /// <summary>
    /// 安全获取 IInteractable 
    /// </summary>
    private IInteractable GetInteractScript(GameObject obj)
    {
        if (obj == null) return null;

        var interactable = obj.GetComponent<IInteractable>();
        if (interactable != null) return interactable;

        interactable = obj.GetComponentInChildren<IInteractable>();
        if (interactable != null) return interactable;

        return null;
    }

    /// <summary>
    /// 应用交互物状态
    /// 应用当前场景交互物上次的最后状态
    /// </summary>
    public void ApplyState()
    {
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();

        foreach(var kvp in _ObjectCache)
        {
            ItemState _object = kvp.Value;

            // 跳过GameObject已被销毁的条目（状态保留，等下次场景重建时恢复）
            if(_object.gameObject == null) continue;

            if(_object.gameObject.scene != currentScene) continue;

            IInteractable interactScript = GetInteractScript(_object.gameObject);

            if(interactScript != null)
            {
                interactScript.CanInteract = _object.isCanInteract;
            }
            else
            {
                Debug.LogWarning($"[交互物全局管理器] 对象 {_object.gameObject.name} 缺失交互组件IInteractable");
            }

            _object.gameObject.SetActive(_object.isVisuable);

            if(isTestState)
            {
                Debug.Log($"[交互物全局管理器] 已应用 {_object.gameObject.name} 的最后状态\n可交互: {_object.isCanInteract}\n隐藏: {_object.isVisuable}");
            }
        }
    }

    /// <summary>
    /// 应用指定场景的待处理状态清单（在缓存构建/重建之后由 OnSceneLoaded 调用）
    /// </summary>
    private void ApplyPendingProcessList(string sceneName)
    {
        if (!_pendingProcessLists.TryGetValue(sceneName, out InteractablesProcessListSO processList))
            return;

        // 应用后移除该待处理项，避免重复应用
        _pendingProcessLists.Remove(sceneName);

        Debug.Log($"[交互物全局管理器] 应用场景 {sceneName} 的待处理状态清单");
        ApplyState(processList);
    }

    /// <summary>
    /// 根据清单应用当前场景的交互物状态
    /// 内部处理：若清单中任一交互物的GameObject引用已失效（场景切换后ReBuildCache尚未执行），
    /// 自动延迟到缓存重建后应用
    /// 外部脚本只需直接调用此方法即可，无需关心时序
    /// </summary>
    /// <param name="processList"></param>
    public void ApplyState(InteractablesProcessListSO processList)
    {
        if(processList == null)
        {
            Debug.LogWarning("[交互物全局管理器] 传入的交互物处理清单为空");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;

        // 检查processList中的交互物是否都已在缓存中且有有效的gameObject引用
        // 如果ReBuildCache还没执行，则缓存中可能：
        //   1) 条目不存在（还没被ReBuildCache添加）
        //   2) 条目存在但gameObject为null（旧的已销毁引用）
        // 两种情况都需要延迟
        bool needDelay = false;
        foreach(var listItem in processList.item)
        {
            if (listItem == null) continue;

            if (!_ObjectCache.TryGetValue(listItem.uniqueID, out ItemState existingState))
            {
                // 缓存中没有该条目 → ReBuildCache还没执行
                needDelay = true;
                break;
            }

            if (existingState.gameObject == null)
            {
                // 条目存在但gameObject已销毁 → ReBuildCache还没执行
                needDelay = true;
                break;
            }
        }

        if (needDelay)
        {
            if (!_pendingProcessLists.ContainsKey(currentSceneName))
            {
                _pendingProcessLists.Add(currentSceneName, processList);
            }
            else
            {
                _pendingProcessLists[currentSceneName] = processList;
            }

            Debug.Log($"[交互物全局管理器] 当前场景 {currentSceneName} 的交互物GameObject尚未重建，已自动延迟应用状态清单");
            return;
        }

        // 缓存有效 → 直接应用
        foreach(var listItem in processList.item)
        {
            if (listItem == null) continue;

            //从缓存中获取该交互物信息
            ItemState itemState;
            if(!_ObjectCache.TryGetValue(listItem.uniqueID , out itemState))
            {
                Debug.LogWarning($"[交互物全局管理器] 在缓存中找不到UniqueID为 {listItem.uniqueID} 的交互物");
                continue;
            }

            //应用状态 并 更新缓存中交互物的状态
            itemState.gameObject.SetActive(listItem.isVisuable);
            itemState.isVisuable = itemState.gameObject.activeSelf;

            IInteractable interactScript = GetInteractScript(itemState.gameObject);

            if(interactScript != null)
            {
                interactScript.CanInteract = listItem.isCanInteract;
                itemState.isCanInteract = interactScript.CanInteract;
            }
            else
            {
                Debug.LogWarning($"[交互物全局管理器] 对象 {itemState.gameObject.name} 缺失交互组件IInteractable");
            }
        }
    }
}