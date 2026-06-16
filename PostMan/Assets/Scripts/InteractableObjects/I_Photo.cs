using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.AudioSystem;

public class I_Photo : MonoBehaviour, IInteractable
{
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    public AudioClip pickSound;

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Photo.cs] 与预知照片交互");

        AudioManager.Instance.Play(AudioTrackId.FX , pickSound);
    }
}
