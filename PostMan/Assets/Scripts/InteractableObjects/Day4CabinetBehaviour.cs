using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class Day4CabinetBehaviour : MonoBehaviour , IInteractable
{
    [Header("SubtitleKey")]
    public string subtitleKey;
    [Header("SFX")]
    public AudioClip knockSFX;

    [Header("启用的SceneOrder")]
    public int sceneOrder;
    [Header("交互设置")]
    public bool triggerOnce;
    private bool haveTrigger = false;
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        if(GetComponent<I_Cabinet>().isOpen)return;

        if(triggerOnce && haveTrigger)return;

        StartCoroutine(Knock());

        haveTrigger = true;
    }

    private IEnumerator Knock()
    {
        GetComponent<I_Cabinet>().stucked = true;

        yield return new WaitForSeconds(2f);

        AudioManager.Instance.Play(AudioTrackId.FX , knockSFX);

        SubtitleUI.Instance.TypeSubtitle(subtitleKey);
    }
}
