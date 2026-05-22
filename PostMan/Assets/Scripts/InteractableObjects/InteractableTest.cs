using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PostMan.Test
{
    public class InteractableTest : MonoBehaviour, IInteractable
    {
        public string idName;
        public bool canInteract;
        public int priority;
        public bool CanInteract { get => canInteract; set => canInteract=value; }
        public int Priority { get => priority; set => priority=value; }

        public void InteractWith(PlayerInteractor player)
        {
            Debug.LogFormat("{0}", idName);
        }
    }
}
