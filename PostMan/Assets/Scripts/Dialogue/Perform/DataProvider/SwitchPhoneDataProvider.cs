using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class SwitchPhoneDataProvider : MonoBehaviour,IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;

        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(SwitchPhonePerform);
        [HideInInspector]
        public I_Telephone phone;
        private void Start()
        {
            phone = this.GetComponent<I_Telephone>();
        }

    }
}
