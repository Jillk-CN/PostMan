using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 用于在交互后禁用可选中物体
    /// </summary>
    public class DisableSelectables : MonoBehaviour, IInteractable
    {
        //要禁用的可交互物体,可以自行拖拽
        private ISelectable[] selectables;

        public bool CanInteract { get => this.enabled; set => this.enabled = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        private void Start()
        {
            selectables = this.GetComponents<ISelectable>();
        }
        public void InteractWith(PlayerInteractor player)
        {
            if (selectables==null)
            {
                return;
            }
            foreach (var selectable in selectables)
            {
                selectable.CanSelect = false;
            }
        }
    }
}
