using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class Day4BoxDoorBehaviour : MonoBehaviour , IInteractable
{
    [Header("离开时门内敲击声触发器")]
    public GameObject doorKnockTrigger;
    [Header("SFX")]
    public AudioClip boxKnockSFX;
    [Header("交互设置")]
    public int sceneOrder;
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    
    public void InteractWith(PlayerInteractor player)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        StartCoroutine(Behaviour());

        doorKnockTrigger.SetActive(true);
    }

    private IEnumerator Behaviour()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.Play(AudioTrackId.FX , boxKnockSFX);

        
    }
}
