using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.AudioSystem;

public class I_Message : MonoBehaviour, IInteractable
{
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    public AudioClip takeSound;

    public void InteractWith(PlayerInteractor player)
    {
        if(CanInteract == false) return;
        if(takeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , takeSound , false , false , 0f , 1f);
        }
        
        //获取当天任务信息
    }
}
