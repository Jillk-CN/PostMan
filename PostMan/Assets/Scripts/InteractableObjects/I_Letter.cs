using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.AudioSystem;

public class I_Letter : MonoBehaviour , IInteractable
{
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    public AudioClip takeSound;
    public AudioClip putSound;
    public AudioClip paperSlideOutSound;

    public void InteractWith(PlayerInteractor player)
    {
        if(CanInteract == false) return;
        if(takeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , takeSound , false , false , 0f , 1f);
        }

        Transform child = gameObject.transform.GetChild(0);
        if(child != null)
        {
            child.gameObject.GetComponent<Renderer>().enabled = false;
        }

        StartCoroutine(DelayDestory());
    }

    //延迟销毁，避免交互系统访问被销毁的组件导致报错
    private IEnumerator DelayDestory()
    {
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    public void PaperSlideOut()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , paperSlideOutSound);
    }
}
