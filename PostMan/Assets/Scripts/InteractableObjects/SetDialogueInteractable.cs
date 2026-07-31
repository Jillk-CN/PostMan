using PostMan.Dialogue;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.InteractableObject
{
    public class SetDialogueInteractable : MonoBehaviour,IInteractable
    {

        [SerializeField]
        private StartDialogueInteractable dialogueInteractable;
        [SerializeField]
        private DialogueSequence dialogue;
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }
        public void InteractWith(PlayerInteractor player)
        {
            dialogueInteractable.SetDialogue(dialogue);        
        }
            


    }
}
