using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class SetObjectInteractable : MonoBehaviour,IInteractable
    {
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        [Tooltip("交互完之后,这些的物体将可以交互")]
        [SerializeField]
        private GameObject[] activeAfterInteract;
        [Tooltip("交互完之后,这些的物体将禁止交互")]
        [SerializeField]
        private GameObject[] inactiveAfterInteract;

        [SerializeField]
        private bool interactOnce=true;//屎山代码
        public void InteractWith(PlayerInteractor player)
        {
            foreach (var obj in activeAfterInteract)
            {
                obj.SetActive(true);
            }
            foreach (var obj in inactiveAfterInteract)
            {
                obj.SetActive(false);
            }
            //出于屎山代码,这里加上这一句
            if (interactOnce)
            {
                this.canInteract = false;
            }
        }
            

    }
}
