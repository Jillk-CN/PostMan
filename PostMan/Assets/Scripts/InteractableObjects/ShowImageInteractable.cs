using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class ShowImageInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Sprite image;
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        public bool showQuitButton = true;
        public void InteractWith(PlayerInteractor player)
        {
            if (image==null)
            {
                return;
            }
            ViewImagePanel.Instance.ShowImage(image,showQuitButton);            
        }
    }
}
