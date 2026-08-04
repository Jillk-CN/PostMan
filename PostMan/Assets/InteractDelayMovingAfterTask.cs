using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class InteractDelayMovingAfterTask : MonoBehaviour , IInteractable
{
    public MovePlayerAfterTask movePlayerAfterTask;

    [Header("交互设置")]
    public int sceneOrder;
    public float delayTime = 0f;
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;
        
        StartCoroutine(DelayRun());
    }

    private IEnumerator DelayRun()
    {
        yield return new WaitForSeconds(delayTime);

        movePlayerAfterTask.DelayMovingPlayerAfterTaskCompletion();
    }
}
