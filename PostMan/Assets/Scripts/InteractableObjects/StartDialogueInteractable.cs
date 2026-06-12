using PostMan.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class StartDialogueInteractable : MonoBehaviour,IInteractable
    {
        [SerializeField]
        private DialogueSequence sequence;
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        public void InteractWith(PlayerInteractor player)
        {
            DialogueManager.Instance.StartDialogue(sequence, null);
        }

    }
}
