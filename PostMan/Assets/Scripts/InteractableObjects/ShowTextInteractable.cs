using Fountain.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class ShowTextInteractable : MonoBehaviour,IInteractable
    {
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        public void InteractWith(PlayerInteractor player)
        {
            TextPanel.Instance.ShowText("a");
        }

    }
}
