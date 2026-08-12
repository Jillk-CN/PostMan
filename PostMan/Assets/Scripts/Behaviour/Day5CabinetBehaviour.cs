using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using PostMan.Scene;
using PostMan.UI;
using UnityEngine;

public class Day5CabinetBehaviour : MonoBehaviour , IInteractable
{
    
    [Header("Animator")]
    public Animator blackHandAnimator;
    
    [Header("SubtitleKey")]
    public string subtitleKey1;
    public string subtitleKey2;
    public string subtitleKey3;
    [Header("SFX")]
    private bool loop = true;
    public AudioClip knock;
    public AudioClip hurryKnock;
    public AudioClip faceJump;
    [Header("启用的SceneOrder")]
    public int sceneOrder;
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    
    private I_Cabinet i_Cabinet;

    void Start()
    {
        Debug.Log("SceneOrder: " + SceneInitializer.Instance.SceneOrder);

        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        i_Cabinet = gameObject.GetComponent<I_Cabinet>();

        StartCoroutine(PlaySound());

        i_Cabinet.OnDoorOpen += OnDoorOpenFirstTime;

        i_Cabinet.OnDoorClose += OnDoorClose;
    }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.Log("SceneOrder: " + SceneInitializer.Instance.SceneOrder);
        
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        loop = false;
    }

    private IEnumerator PlaySound()
    {
        while(loop && !i_Cabinet.isOpen)
        {
            yield return new WaitForSeconds(1f);

            AudioManager.Instance.Play(AudioTrackId.FX , knock);
        }
    }

    private void OnDoorOpenFirstTime()
    {
        StopCoroutine(PlaySound());

        SubtitleUI.Instance.TypeSubtitle(subtitleKey1 , 1f);

        i_Cabinet.OnDoorOpen -= OnDoorOpenFirstTime;
    }

    private void OnDoorClose()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , hurryKnock);

        SubtitleUI.Instance.TypeSubtitle(subtitleKey2);

        i_Cabinet.OnDoorClose -= OnDoorClose;

        i_Cabinet.OnDoorOpen += OnDoorOpenSecondTime;
    }

    private void OnDoorOpenSecondTime()
    {
        StartCoroutine(FaceJump());

        i_Cabinet.OnDoorOpen -= OnDoorOpenSecondTime;
    }

    private IEnumerator FaceJump()
    {
        yield return new WaitForSeconds(1f);
        
        AudioManager.Instance.Play(AudioTrackId.FX , faceJump);

        blackHandAnimator.SetTrigger("PAT");

        yield return new WaitForSeconds(3f);

        BlackScreen.Instance.BlackInOut("" , 0.1f , 0.3f , 0.1f);

        yield return new WaitForSeconds(0.9f);

        StartCoroutine(i_Cabinet.CloseDoor());

        blackHandAnimator.ResetTrigger("PAT");

        SubtitleUI.Instance.TypeSubtitle(subtitleKey3 , 3f);
    }
}
