using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    public class DirectlySetInteractableTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool interactableOnTrigger;
        [SerializeField]
        private bool selectableOnTrigger;
        [SerializeField]
        [Tooltip("要设置的可交互物体")]
        //假定一个物体上的可交互物体是同时启用同时禁用的,否则因作为另一个物体
        private GameObject[] interactableObjects;
        [SerializeField]
        [Tooltip("要设置的可交互物体")]
        private GameObject[] selectableObjects;
        [SerializeField]
        private bool triggerOnce=true;
        [SerializeField]
        private bool canTrigger=true;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")||!canTrigger)
            {
                return;
            }
            if (triggerOnce) 
            {
                this.canTrigger = false;
            } 
            IInteractable[] interactables;
            for (int i = 0; i < this.interactableObjects.Length; i++)
            {
                interactables = this.interactableObjects[i].GetComponents<IInteractable>();
                foreach (var item in interactables)
                {
                    item.CanInteract = interactableOnTrigger;
                }
            }
            ISelectable[] selectables;
            for (int i = 0; i < selectableObjects.Length; i++)
            {
                selectables = selectableObjects[i].GetComponents<ISelectable>();
                foreach (var item in selectables)
                {
                    item.CanSelect = selectableOnTrigger;
                }
            }
            this.gameObject.SetActive(false);//屎山代码
        }
    }
}
