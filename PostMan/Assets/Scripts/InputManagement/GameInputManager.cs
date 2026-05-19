using PostMan.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PostMan.InputManagement
{
    /// <summary>
    /// 输入管理器(单例),管理所有的InputSource,提供InputSource需要的数据,
    /// 并提供一些方便的方法
    /// 注:InputAction有关的InputSource和manager挂在同一个物体上
    /// </summary>
    public class GameInputManager :MonoSingleton<GameInputManager> 
    {
        /// <summary>
        /// 简单地放在这里先,虽然作为这样的管理器放这里并不合适
        /// </summary>
        private PlayerInputActions inputActions;
        /// <summary>
        /// 整个游戏的InputProviders
        /// </summary>
        private List<IInputSource> sources;

        protected override void Init()
        {
            base.Init();
            if (Instance!=this)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
            inputActions = new PlayerInputActions();

            sources = this.GetComponents<IInputSource>().ToList();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //this.GetInputSystemSource<PlayerSightInputSource>
        }
        private void OnDestroy()
        {
            inputActions.Dispose();
            inputActions = null;
        }

        /// <summary>
        /// 获得输入系统的引用,不要自行new一个新的实例
        /// </summary>
        /// <returns></returns>
        public PlayerInputActions GetInputAction()
        {
            return this.inputActions;
        }
        /// <summary>
        /// 获得全局唯一与InputSystem有关的输入源,
        /// 其它的输入源暂时不被这个Manager管理
        /// </summary>
        /// <typeparam name="T">InputSource对应的类型</typeparam>
        /// <returns></returns>
        public T GetInputSystemSource<T>()where T : IInputSource
        {
            return (T)sources.Find((arg) =>
            {
                return arg is T;
            });
        }
        /// <summary>
        /// 快速启用/禁用InputSystem输入源 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enabled"></param>
        public void SetInputSystemSource<T>(bool enabled)where T :IInputSource
        {
            IInputSource src =GetInputSystemSource<T>();
            if (enabled)
            {
                src?.Enable();
            }
            else
            {
                src?.Disable();
            }
        }

        /// <summary>
        /// 隐藏鼠标
        /// </summary>
        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        /// <summary>
        /// 显示鼠标
        /// </summary>
        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
