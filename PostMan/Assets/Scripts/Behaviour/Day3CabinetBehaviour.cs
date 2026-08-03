using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class Day3CabinetBehaviour : MonoBehaviour , IInteractable
{
    private I_Cabinet i_Cabinet;
    private bool isOpen;
    [Header("字幕Key")]
    public string subtitle1;
    public string subtitle2;
    [Header("SFX")]
    public AudioClip sfx;
    [Header("启用的SceneOrder")]
    public int sceneOrder;
    [Header("交互设置")]
    public bool triggerOnce;
    private bool haveTrigger = false;
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    
    void Start()
    {
        i_Cabinet = GetComponent<I_Cabinet>();
    }


    public void InteractWith(PlayerInteractor player)
    {
        if(SceneInitializer.Instance.SceneOrder != sceneOrder)return;

        if(triggerOnce && haveTrigger)return;
        
        if(i_Cabinet.isOpen)
        {
            StartCoroutine(Behaviour());
        }
        
        haveTrigger = true;
    }

    private IEnumerator Behaviour()
    {
        SubtitleUI.Instance.TypeSubtitle(subtitle1);

        yield return new WaitForSeconds(0.3f);

        AudioManager.Instance.Play(AudioTrackId.FX , sfx);

        yield return new WaitForSeconds(1f);

        SubtitleUI.Instance.TypeSubtitle(subtitle2);

        yield return new WaitForSeconds(2.5f);

        AudioManager.Instance.Play(AudioTrackId.FX , sfx);
    }
}
