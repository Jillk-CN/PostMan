using PostMan.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueSequence dialogue;
    public bool triggerOnce=true;
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
