using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class LookDataProvider : MonoBehaviour,IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;
        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(LookAtPerform);
        [HideInInspector]
        public PlayerMotion motion;
        public Transform target;
        public float lookDuration = 1f;
        private void Start()
        {
            motion = PlayerInstance.Instance.GetComponent<PlayerMotion>();
        }

    }
}
