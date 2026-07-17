using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    public class SetOtherInteractableTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool triggerOnce = true;
        [SerializeField]
        private bool canTrigger = true;
        [SerializeField]
        private InteractablesProcessListSO interactables;//暂时这样
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")&&canTrigger)
            {
                InteractableManager.Instance.ApplyState(interactables);
                if (triggerOnce) 
                {
                    this.canTrigger = false;
                } 
            }
        }

    }
}
