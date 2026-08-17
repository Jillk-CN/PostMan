
using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class InteractableSFXSwitcher : MonoBehaviour , IInteractable
{
    [Header("生效的SceneOrder")]
    public int sceneOrder = 0;
    [Header("SFX")]
    public AudioClip sfx;
    [Header("延迟时长")]
    public float delayTime = 0f;
    [Header("交互设置")]
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

        if(sfx == null)
        {
            Debug.LogError(gameObject.name + ": 音效为空");
            return;
        }

        StartCoroutine(DelayPlayAudio());
        
        haveTrigger = true;
        
    }

    private IEnumerator DelayPlayAudio()
    {
        yield return new WaitForSeconds(delayTime); 
        AudioManager.Instance.Play(AudioTrackId.FX , sfx , false ,false , 0f , 1f);
    }
}
