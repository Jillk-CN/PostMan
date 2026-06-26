using PostMan.InputManagement;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePlayer : MonoBehaviour
{
        //要禁用的可交互物体,可以自行拖拽
        private IInteractable[] limitedInteractables;
        [SerializeField]
        private bool canInteract=true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }

        [SerializeField]
        private int priority = -1;
        public int Priority { get => priority; set => priority = value; }

        private void Start()
        {
            limitedInteractables = this.GetComponents<IInteractable>();
        }

        public void InteractWith(PlayerInteractor player)
        {
        GameInputManager.Instance.SetPlayerAllInput(false);
        }
}
