using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using UnityEngine;

public class SubtitleInteractTrigger : MonoBehaviour , IInteractable
{
    public string subtitleKey;

    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        SubtitleUI.Instance.TypeSubtitle(subtitleKey);
    }
}
