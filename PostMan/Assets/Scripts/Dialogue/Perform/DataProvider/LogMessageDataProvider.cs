using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class LogMessageDataProvider :MonoBehaviour, IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;
        public string debugMessage;

        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(LogMessagePerform);
        
    }
}
