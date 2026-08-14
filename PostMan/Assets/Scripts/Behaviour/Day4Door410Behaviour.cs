using System.Collections;
using System.Collections.Generic;
using PostMan.InteractableObject;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class Day4Door410Behaviour : MonoBehaviour , IInteractable
{
    public I_Door_S i_Door_S;
    public MovePlayerAfterTask movePlayerAfterTask;
    public InteractDelayMovingAfterTask interactDelayMovingAfterTask;

    [Header("交互设置")]
    public int SceneOrder;
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    private bool hasInteract = false;

    void OnEnable()
    {
        GameSceneManager.OnSceneSwitchCompleted += OnSceneSwitchCompleted;
        TryActivate();
    }

    void OnDisable()
    {
        GameSceneManager.OnSceneSwitchCompleted -= OnSceneSwitchCompleted;
    }

    private void OnSceneSwitchCompleted(IReadOnlyList<string> loadedScenes)
    {
        TryActivate();
    }

    /// <summary>
    /// SceneOrder 是在场景加载完成、OnSceneSwitchCompleted 触发后才 +1 的，
    /// 所以不能在 Awake/Start/OnEnable 里直接判断；在事件回调里判断才能拿到本场景的次序。
    /// </summary>
    private void TryActivate()
    {
        if(SceneOrder != SceneInitializer.Instance.SceneOrder)return;

        i_Door_S._PlaceBox = true;
    }

    public void InteractWith(PlayerInteractor player)
    {
        if(SceneOrder != SceneInitializer.Instance.SceneOrder || hasInteract || i_Door_S._PlaceBox)return;
        
        interactDelayMovingAfterTask.Cancel();

        StartCoroutine(DelaySetCheckDoor());
        
        hasInteract = true;
    }
    
    private IEnumerator DelaySetCheckDoor()
    {
        //等 PlaceBox 演出完全结束（运行锁释放）再置 _CheckDoor，
        //避免 SquatDown 在 PlaceBox 还在播放时叠加启动
        while (i_Door_S.IsPlaceBoxRunning)
        {
            yield return null;
        }

        i_Door_S._CheckDoor = true;
    }
}
