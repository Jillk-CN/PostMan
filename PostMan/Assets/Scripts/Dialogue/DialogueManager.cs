using PostMan.Common;
using PostMan.InputManagement;
using PostMan.Player;
using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PostMan.Dialogue
{
    /*对话系统框架示例图:这是一个相当简陋的框架

     发起对话的对象(比如npc)--(传递数据)-->DialogueManager--->DialoguePanel(UI)显示对话
         ↑                                               ↓
         ↑                                               ↓
 挂有SO,以及IPerformDataProvider实现类,                获得DialoguePerform实现类对象,
    提供数据供DialoguePerform实现类使用                    传递数据,调用接口来实现简单的演出
     */    

    /// <summary>
    /// 对话管理器(单例),提供开启对话、继续对话的接口,并调用对话中的演出事件
    /// </summary>
    public class DialogueManager :MonoSingleton<DialogueManager>
    {
        #region 对话文本显示相关

        private DialoguePanel dialoguePanel;
        /// <summary>
        /// 当前进行的对话的SO
        /// </summary>
        private DialogueSequence dialogueSO;
        /// <summary>
        /// 实际用来显示对话的文本序列
        /// </summary>
        private List<DialogueSequence.DialogueInfo> dialogueSequence;
        /// <summary>
        /// 对话是否结束
        /// </summary>
        private bool dialogueCompleted=true;
        /// <summary>
        /// 当前对话的索引
        /// </summary>
        private int dialogueIndex;
        #endregion

        #region 演出相关
        /// <summary>
        /// 当没有中间的演出时,只可能末尾有演出时,会给perfomIndex赋值
        /// </summary>
        private const int NO_MIDDLE_PERFORM = -2;
        /// <summary>
        /// 演出数据
        /// </summary>
        private List<IPerformDataProvider> performDatas;
        private DialoguePerformFactory performFactory;
        #endregion

        //输入来源
        private DialogueInputSource dialogueInput;

        private void Start()
        {
            dialoguePanel = DialoguePanel.Instance;
            performFactory = new DialoguePerformFactory();

            dialogueInput = GameInputManager.Instance.
                GetInputSystemSource<DialogueInputSource>();

        }

        private void Update()
        {
            if (dialogueInput.GetContinue())
            {
                ContinueDialogue();
            }    
        }
        /// <summary>
        /// 开始对话 
        /// </summary>
        /// <param name="dialogues">对话数据的SO</param>
        /// <param name="dataProviders">提供演出数据的对象</param>
        public void StartDialogue(DialogueSequence dialogues, List<IPerformDataProvider> dataProviders)
        {
            if (dialogues==null)
            {
                return;
            }
            if (!dialogueCompleted)
            {
                return;
            }

            //记录对话数据
            dialogueIndex = -1;
            dialogueCompleted = false;
            this.dialogueSO = dialogues;
            this.dialogueSequence = dialogueSO.GetSequence();

            //处理演出数据
            //按照在哪个对话进行演出,以及在同一个对话演出时的优先级来进行排序
            this.performDatas = dataProviders.
                OrderBy((data) => data.TargetDialogueIndex).
                ThenByDescending((data) => data.Priority).ToList();

            //禁用输入,注意对话时不允许打开设置面板,会有bug,要修有点麻烦
            GameInputManager.Instance.DisablePlayerAllInput();
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(false);
            dialogueInput.Enable();

            //UI显示
            dialoguePanel.SetVisible(true);
            ContinueDialogue();
        }
        /// <summary>
        /// 继续当前已经开始的对话
        /// </summary>
        private void ContinueDialogue()
        {
            if (this.dialogueSO==null)
            {
                return;
            }
            //对话在显示中则跳过显示
            if (dialoguePanel.IsShowing())
            {
                dialoguePanel.Skip();
                return;
            }

            //否则到下一句对话
            dialogueIndex++;
            //没有对话了就结束
            if (dialogueIndex>=this.dialogueSequence.Count)
            {
                EndDialogue();
                return;
            }
            
            DialogueSequence.DialogueInfo dialogueInfo =dialogueSequence[dialogueIndex];
            dialoguePanel.ShowDialogue(dialogueInfo);
            TriggerPerform(dialogueIndex);
        }
        private void EndDialogue()
        {
            dialogueCompleted = true;
            dialoguePanel.SetVisible(false);
            //触发结束时的演出
            TriggerPerform(IPerformDataProvider.END_INDEX);

            //恢复输入
            // 对话结束，禁用对话推进输入
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(true);
            dialogueInput.Disable();

            GameInputManager.Instance.EnablePlayerAllInput();

            this.dialogueSO = null;
            this.performDatas = null;
        }
        /// <summary>
        /// 实现演出效果
        /// </summary>
        /// <param name="dialogueIndex">这个对话在序列里的索引</param>
        private void TriggerPerform(int dialogueIndex)
        {
            if (this.performDatas==null)
            {
                return;
            }

            //找到对话演出要用到的数据,依次触发
            int dataIndex = performDatas.FindIndex
                ((data) => { return data.TargetDialogueIndex == dialogueIndex; });
            if (dataIndex==-1)
            {
                return;
            }
            DialoguePerform perform;
            while ( (dataIndex < performDatas.Count) &&
                (performDatas[dataIndex].TargetDialogueIndex == dialogueIndex)) 
            {
                perform = performFactory.GetPerform(performDatas[dataIndex].PerformName);
                perform.ReceiveData(performDatas[dataIndex]);
                perform.Perform();
                dataIndex++;

            } 
        }
    }
}
