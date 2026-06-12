using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    /// <summary>
    /// 对话演出类基类,负责实现对话过程里要实现的简单演出,比如人物移动,播放动画/音效等等,
    /// 通常一个实现类就可以完成一句对话的演出,更复杂的演出就不太可能用这个设计了
    /// 实现类命名规范:"Foutain.Dialogue.+实现类名称(以Perform结尾)"
    /// </summary>
    public abstract class DialoguePerform
    {
        /// <summary>
        /// 传递数据,实现演出的时候要先调用这个方法!
        /// </summary>
        /// <param name="data"></param>
        public abstract void ReceiveData(IPerformDataProvider data);
        /// <summary>
        /// 实现演出效果
        /// </summary>
        public abstract void Perform();
    }
}
