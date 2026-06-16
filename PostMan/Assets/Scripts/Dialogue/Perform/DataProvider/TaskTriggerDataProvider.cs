using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class TaskTriggerDataProvider : MonoBehaviour,IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;
        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(StartTaskPerform);

        [HideInInspector]
        public DialogueTaskTrigger trigger;
        private void Awake()
        {
            trigger = this.GetComponent<DialogueTaskTrigger>();            
        }
    }
}
