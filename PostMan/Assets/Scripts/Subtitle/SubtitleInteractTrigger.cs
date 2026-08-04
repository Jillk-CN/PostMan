using System;
using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class SubtitleInteractTrigger : MonoBehaviour , IInteractable
{
    public string subtitleKey;
    public float delayTime = 0f;
    [Header("启用的SceneOrder")]
    public int sceneOrder;
    public bool triggerOnce;
    private bool haveTrigger = false;

    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        if(triggerOnce && haveTrigger)return;

        if(SceneInitializer.Instance.SceneOrder != sceneOrder)return;

        //StartCoroutine(TypeDelay());
        SubtitleUI.Instance.TypeSubtitle(subtitleKey,delayTime);
        haveTrigger = true;
    }

    private IEnumerator TypeDelay()
    {
        yield return new WaitForSeconds(delayTime);
        SubtitleUI.Instance.TypeSubtitle(subtitleKey);
    }
}
