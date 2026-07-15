using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class SetObjectDataProvider : MonoBehaviour,IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;
        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(SetObjectPerform);

        public GameObject target;
        public Vector3 position;
        public bool active;
    }
}
