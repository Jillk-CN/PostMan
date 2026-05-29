using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 如果要限制指定Interactable的交互,只允许一次,挂这个脚本
    /// </summary>
    public class LimitInteract : MonoBehaviour, IInteractable
    {
        //要禁用的可交互物体,可以自行拖拽
        private IInteractable[] limitedInteractables;
        [SerializeField]
        private bool canInteract=true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }

        [SerializeField]
        [Tooltip("注意,优先级要是最低的")]
        private int priority = -1;
        public int Priority { get => priority; set => priority = value; }

        private void Start()
        {
            limitedInteractables = this.GetComponents<IInteractable>();
        }

        public void InteractWith(PlayerInteractor player)
        {
            if(limitedInteractables==null)
            {
                this.CanInteract = false;
                return;
            }
            foreach (var interactable in limitedInteractables)
            {
                interactable.CanInteract = false;
            }
            this.CanInteract = false;            

        }
    }
}
