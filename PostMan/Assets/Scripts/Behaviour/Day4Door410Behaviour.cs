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

    void Start()
    {
        if(SceneOrder != SceneInitializer.Instance.SceneOrder)return;

        i_Door_S._PlaceBox = true;
    }

    public void InteractWith(PlayerInteractor player)
    {
        if(SceneOrder != SceneInitializer.Instance.SceneOrder || hasInteract || i_Door_S._PlaceBox)return;

        i_Door_S._CheckDoor = true;
        interactDelayMovingAfterTask.Cancel();

        hasInteract = true;
    }
}
