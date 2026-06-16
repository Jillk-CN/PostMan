using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    /// <summary>
    /// 为对话演出提供数据的接口,它的子类获取数据供DialoguePerform的子类使用,
    /// 实现类自行做强制类型转换
    /// </summary>
    public interface IPerformDataProvider 
    {
        public const int END_INDEX = -1;
        /// <summary>
        /// 返回的索引标识在那句对话执行,如果是-1,说明是结束时用的演出
        /// </summary>
        public int TargetDialogueIndex { get; }
        /// <summary>
        /// 由于要简单地组合出演出效果,因此需要优先级,
        /// 越大越先执行
        /// </summary>
        public int Priority { get; }
        /// <summary>
        /// 数据类对应的演出类的名称
        /// </summary>
        public string PerformName { get; }
    }
}
