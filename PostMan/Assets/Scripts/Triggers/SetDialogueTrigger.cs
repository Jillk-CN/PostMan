using PostMan.Dialogue;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    public class SetDialogueTrigger : MonoBehaviour
    {

        [SerializeField]
        private StartDialogueInteractable dialogueInteractable;
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
                dialogueInteractable.SetDialogue(dialogue);        
                if (triggerOnce) 
                {
                    this.canTrigger = false;
                } 
            }
        }
    }
}
