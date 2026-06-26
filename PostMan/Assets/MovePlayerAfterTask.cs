using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using UnityEngine;

public class MovePlayerAfterTask : MonoBehaviour
{
    [Header("场景配置")]

    /// <summary>需要加载的场景地址列表（Addressable key）。</summary>
    [Tooltip("触发后需要加载的场景（Addressable 地址）")]
    [SerializeField] private List<string> _scenesToLoad = new List<string>();

    /// <summary>需要卸载的场景地址列表（Addressable key）。</summary>
    [Tooltip("触发后需要卸载的场景（Addressable 地址）")]
    [SerializeField] private List<string> _scenesToUnload = new List<string>();

    public int taskIndex; // 任务索引   
    public Vector3 targetPosition;

    public TaskSO task;

    public void OnEnable()
    {
        TaskEventBus.OnTaskCompleted += MovePlayerToPositionAfterTaskCompletion;
    }

    public void OnDisable()
    {
        TaskEventBus.OnTaskCompleted -= MovePlayerToPositionAfterTaskCompletion;
    }

    public void MovePlayerToPositionAfterTaskCompletion(TaskRuntimeData taskData)
    {
        if (taskData.Definition.taskIndex == taskIndex)
        {
            TaskManager.Instance.StartTask(task);
            PlayerInstance.Instance.transform.position = targetPosition;
            GameSceneManager.Instance.SwitchScenes(_scenesToLoad, _scenesToUnload);
        }
    }
}
