using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.AudioSystem;

public class I_Piece : MonoBehaviour, IInteractable
{
    public AudioClip takeSound;

    [Header("要推进的任务索引")]
    [SerializeField] private int taskIndex;
    
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        if(CanInteract == false) return;


        if(takeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , takeSound);
        }
    }
}
