using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class SetOtherInteractables : MonoBehaviour,IInteractable
    {
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        /*
        [Tooltip("交互完之后,这些的物体将可以交互")]
        private int onAfterInteract;
        [Tooltip("交互完之后,这些的物体将禁止交互")]
        private int offAfterInteract;
         */
        [Header("初始化参数,编辑器内手动配置")]
        public InteractablesProcessListSO processListSO;
        public void InteractWith(PlayerInteractor player)
        {
            //出于某些考量,用这个<
            if (processListSO==null)
            {
                return;
            }
            InteractableManager.Instance.ApplyState(processListSO);                    
        }
            
    }
}
