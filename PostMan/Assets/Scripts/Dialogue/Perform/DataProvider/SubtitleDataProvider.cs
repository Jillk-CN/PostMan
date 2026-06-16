using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class SubtitleDataProvider : MonoBehaviour, IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;
        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(ShowSubtitlePerform);

        [Tooltip("字幕的键")]
        public string subtitleKey;

    }
}
