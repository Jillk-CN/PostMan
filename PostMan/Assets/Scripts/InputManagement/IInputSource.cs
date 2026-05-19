using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.InputManagement
{
    /// <summary>
    /// 输入源接口,实现类负责提供输入
    /// </summary>
    public interface IInputSource
    {
        /// <summary>
        /// 这个输入源正在启用中?
        /// </summary>
        public bool Enabled { get; }
        public void Enable();
        public void Disable();
    }
}
