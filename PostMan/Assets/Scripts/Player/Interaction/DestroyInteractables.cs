using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using UnityEngine;

public class DestroyInteractables : MonoBehaviour , IInteractable
{
    private bool canInteract = true;
    public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
    [SerializeField]
    private int priority;
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        Destroy(gameObject);
    }
}
