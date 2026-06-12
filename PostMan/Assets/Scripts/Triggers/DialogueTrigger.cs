using PostMan.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField]
        private DialogueSequence dialogue;
        [SerializeField]
        private bool triggerOnce=true;
        [SerializeField]
        private bool canTrigger=true;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")&&canTrigger)
            {
                DialogueManager.Instance.StartDialogue(dialogue, null);
                if (triggerOnce) 
                {
                    this.canTrigger = false;
                } 
            }
        }
    }
}
