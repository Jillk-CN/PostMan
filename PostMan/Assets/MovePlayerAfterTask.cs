using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using UnityEngine;
using PostMan.UI;

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

    [SerializeField] private bool _controlByOutside = false; // 是否由外部触发玩家移动
    [SerializeField] private bool _movePlayerAfterTaskCompletion = true; // 是否在任务完成后移动玩家
    [SerializeField] private bool _rotatePlayerAfterTaskCompletion = false; // 是否在任务完成后旋转玩家
    public Vector3 targetPosition;
    public Vector3 targetRotation;

    public TaskSO task;
    private TaskRuntimeData taskDataRecorded;

    public void OnEnable()
    {
        if (_controlByOutside == false)
        {
            TaskEventBus.OnTaskCompleted += MovePlayerToPositionAfterTaskCompletion;
        }
        else
        {
            TaskEventBus.OnTaskCompleted += RecordTaskData;
        }
    }

    public void OnDisable()
    {
        if (_controlByOutside == false)
        {
            TaskEventBus.OnTaskCompleted -= MovePlayerToPositionAfterTaskCompletion;
        }
        else
        {
            TaskEventBus.OnTaskCompleted -= RecordTaskData;
        }
    }

    /// <summary>
    /// 记录匹配 taskIndex 的任务完成数据，供外部延迟调用。
    /// </summary>
    public void RecordTaskData(TaskRuntimeData taskData)
    {
        if (taskData.Definition.taskIndex == taskIndex)
        {
            taskDataRecorded = taskData;
        }
    }

    /// <summary>
    /// 外部调用入口：延迟触发场景切换和玩家移动。
    /// </summary>
    public void DelayMovingPlayerAfterTaskCompletion()
    {
        if (taskDataRecorded == null)
            return;

        MovePlayerToPositionAfterTaskCompletion(taskDataRecorded);
        taskDataRecorded = null; // 使用后清除，避免外部重复调用导致重复触发
    }

    public void MovePlayerToPositionAfterTaskCompletion(TaskRuntimeData taskData)
    {
        if (taskData.Definition.taskIndex == taskIndex)
        {
            if (_movePlayerAfterTaskCompletion)
            {
                BlackScreen.Instance?.BlackIn("", 1f, () =>
                {
                    GameSceneManager.Instance.SwitchScenes(_scenesToLoad, _scenesToUnload, targetPosition);
                    BlackScreen.Instance?.BlackOut("", 1f);
                });
            }

            if (_rotatePlayerAfterTaskCompletion)
            {
                PlayerInstance.Instance.transform.rotation = Quaternion.Euler(targetRotation);
            }

            TaskManager.Instance.StartTask(task);
        }
    }
}