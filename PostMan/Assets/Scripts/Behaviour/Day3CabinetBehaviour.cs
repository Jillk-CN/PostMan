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
    
    private bool hasStarted = false;

    void OnEnable()
    {
        GameSceneManager.OnSceneSwitchCompleted += OnSceneSwitchCompleted;
        TryStart();
    }

    void OnDisable()
    {
        GameSceneManager.OnSceneSwitchCompleted -= OnSceneSwitchCompleted;
    }

    private void OnSceneSwitchCompleted(IReadOnlyList<string> loadedScenes)
    {
        TryStart();
    }

    
    private void TryStart()
    {
        if(hasStarted) return;

        if(SceneInitializer.Instance.SceneOrder != sceneOrder)return;
        
        i_Cabinet = GetComponent<I_Cabinet>();

        i_Cabinet.OnDoorOpen += OnOpen;
        i_Cabinet.OnDoorClose += OnClose;

        hasStarted = true;
    }


    public void InteractWith(PlayerInteractor player)
    {
        if(SceneInitializer.Instance.SceneOrder != sceneOrder)return;

        //只有门开着时才算触发；门没开时直接返回，且不能把 haveTrigger 置为 true。
        //否则“开门”那一次交互会把 haveTrigger 标记掉，门开后再交互会被 triggerOnce
        //提前拦截，Behaviour()（两段音效+字幕）永远执行不到。
        if(i_Cabinet == null || !i_Cabinet.isOpen)return;

        if(triggerOnce && haveTrigger)return;

        
        haveTrigger = true;
    }

    private IEnumerator Behaviour()
    {
        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.Play(AudioTrackId.FX , sfx);

        yield return new WaitForSeconds(1f);

        SubtitleUI.Instance.TypeSubtitle(subtitle2);

        yield return new WaitForSeconds(5f);

        AudioManager.Instance.Play(AudioTrackId.FX , sfx);

        i_Cabinet.OnDoorOpen -= OnOpen;
        i_Cabinet.OnDoorClose -= OnClose;
    }

    private void OnOpen()
    {
        SubtitleUI.Instance.TypeSubtitle(subtitle1);
    }

    private void OnClose()
    {
        StartCoroutine(Behaviour());
    }
}
