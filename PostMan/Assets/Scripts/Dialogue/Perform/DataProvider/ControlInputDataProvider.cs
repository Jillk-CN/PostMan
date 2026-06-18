using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class ControlInputDataProvider : MonoBehaviour,IPerformDataProvider
    {
        [SerializeField]
        private int priority=100;
        [SerializeField]
        private int dialogueIndex;

        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(ControlInputPerform);

        [Tooltip("禁用输入或启用输入")]
        public bool enableInput=false;
        //暂时没有什么简便的方法传递InputSource,Perform那边先硬编码先

    }
}
