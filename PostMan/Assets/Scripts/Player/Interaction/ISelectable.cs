using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 选中效果的接口,实现类实现选中的逻辑,比如显示相关的视觉效果
    /// </summary>
    public interface ISelectable 
    {
        /// <summary>
        /// 当前物体是否被选中
        /// </summary>
        public bool Selected { get; set; }
        /// <summary>
        /// 选中物体
        /// </summary>
        public void Select();
        /// <summary>
        /// 取消选中物体
        /// </summary>
        public void Deselect();
    }
}
