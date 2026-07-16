using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using UnityEngine;

/// <summary>
/// 通过交互触发交互物状态更新
/// </summary>
public class IMInteractTrigger : MonoBehaviour , IInteractable
{
    [Header("延迟时长")]
    public float time = 0f;

    [Header("交互物处理清单SO")]
    public InteractablesProcessListSO interactablesProcessListSO;

    [Header("在哪个任务里生效（填任务索引）")]
    public int taskIndex;

    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        if(!TaskManager.Instance.IsTaskActive(taskIndex))return;

        StartCoroutine(ApplyList());
    }

    private IEnumerator ApplyList()
    {
        yield return new WaitForSeconds(time);

        InteractableManager.Instance.ApplyState(interactablesProcessListSO);
    }
}
