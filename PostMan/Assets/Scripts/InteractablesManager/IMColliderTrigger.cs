using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过触发触发器来更新交互物状态
/// </summary>
public class IMColliderTrigger : MonoBehaviour
{
    [Header("延迟时长")]
    public float time = 0f;

    [Header("交互物处理清单SO")]
    public InteractablesProcessListSO interactablesProcessListSO;

    [Header("在哪个任务里生效（填任务索引）")]
    public int taskIndex;

    void OnTriggerEnter(Collider other)
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
