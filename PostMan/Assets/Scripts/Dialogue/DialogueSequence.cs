using PostMan.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace PostMan.Dialogue
{
    /// <summary>
    /// 对话序列SO,存储完整的对话
    /// </summary>
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Sequence")]
    public class DialogueSequence :ScriptableObject 
    {
        /// <summary>
        /// 对话信息,存储“一行”对话,对话系统里的基本数据
        /// </summary>
        public class DialogueInfo
        {
            private string speaker;
            private string text;
            public DialogueInfo(string speaker, string text)
            {
                this.speaker = speaker;
                this.text = text;
            }
            public string GetSpeaker()
            {
                return speaker;
            }
            public string GetText()
            {
                return text;
            }
        }

        [Tooltip("对话的文本的键,按顺序写入")]
        public string[] dialogueKeys;

        [Tooltip("对话唯一的数字id,仅用于标识,数字之间没有任何关联")]
        public int id;
        //private List<DialogueInfo> sequence;
        [Tooltip("对话结束后的演出设置")]
        public string dialogueEndPerformName;
        /// <summary>
        /// 获取每一句对话的数组
        /// </summary>
        /// <returns></returns>
        public List<DialogueInfo> GetSequence()
        {
            return ConstructSequence(this.dialogueKeys);
        }
        private List<DialogueInfo> ConstructSequence(string[] dialogueKeys)
        {
            List<DialogueInfo> sequence = new List<DialogueInfo>();
            string speaker;
            string[] dialogues;
            DialogueInfo[] infos;
            string processText;
            for (int keyIndex = 0; keyIndex < dialogueKeys.Length; keyIndex++)
            {
                //获得文本
                processText = LocalizationManager.Instance.
                    GetLocalizedString
                    (LocalizationManager.TableName.DialogueTable,dialogueKeys[keyIndex]);
                LocalizationManager.Instance.
                    GetLocalizedDialogue(processText, out speaker, out dialogues);

                //构造数据
                infos = new DialogueInfo[dialogues.Length];
                for (int i = 0; i < infos.Length; i++)
                {
                    infos[i] = new DialogueInfo(speaker, dialogues[i]);
                }
                sequence.AddRange(infos);
            }
            return sequence;
        }
        //public bool unlockInput=true;

        //可选:添加对话开始时的事件和对话结束时的事件,
        //对话不一定于策划案的划分一致,有些对话可能要拆成多个对话
    }
}
