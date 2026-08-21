using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using PostMan.Scene;
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

    [Header("在哪个SceneOrder里生效")]
    public int sceneOrder;

    [Header("音效")]
    public AudioClip sfx;

    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    public bool _InteractOnce = false;
    private bool hasInteract = false;

    public void InteractWith(PlayerInteractor player)
    {
        if(SceneInitializer.Instance.SceneOrder != sceneOrder || hasInteract)return;

        StartCoroutine(ApplyList());

        if(_InteractOnce)
        {
            hasInteract = true;
        }
    }

    private IEnumerator ApplyList()
    {
        yield return new WaitForSeconds(time);

        if(sfx != null)AudioManager.Instance.Play(AudioTrackId.FX , sfx);

        InteractableManager.Instance.ApplyState(interactablesProcessListSO);
    }
}
