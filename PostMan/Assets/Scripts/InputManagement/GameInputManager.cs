using PostMan.Common;
using PostMan.Dialogue;
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
            if (Instance != this)
            {
                Destroy(this.gameObject);
                return; // 重复实例提前返回，避免继续初始化
            }
            DontDestroyOnLoad(this.gameObject);
            inputActions = new PlayerInputActions();

            sources = this.GetComponents<IInputSource>().ToList();

            // 场景切换后重新绑定：所有 InputSource 重新获取当前 inputActions 引用
            foreach (var src in sources)
            {
                src.RefreshInputActions();
            }

            HideCursor();
        }
        private void OnDestroy()
        {
            inputActions?.Dispose(); // null 检查：重复实例被 Destroy 时 inputActions 可能未赋值
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
            if (src==null)
            {
                return;
            }
            if (enabled&&!src.Enabled)
            {
                src.Enable();
            }
            else if(!enabled&&src.Enabled)
            {
                src.Disable();
            }
        }
        public void SetPlayerAllInput(bool enable)
        {
            SetInputSystemSource<PlayerMotionInputSource>(enable);
            SetInputSystemSource<PlayerSightInputSource>(enable);
            SetInputSystemSource<PlayerInteractInputSource>(enable);
        }

        /// <summary>
        /// 将所有输入源重置为游戏场景默认状态。
        /// 在每次加载游戏场景后调用，防止上一次游戏会话中的启用/禁用状态污染新会话。
        /// </summary>
        public void ResetToGameDefaults()
        {
            // 玩家核心输入：默认全部启用
            SetInputSystemSource<PlayerMotionInputSource>(true);
            SetInputSystemSource<PlayerSightInputSource>(true);
            SetInputSystemSource<PlayerInteractInputSource>(true);

            // 暂停键：游戏开始即可用
            SetInputSystemSource<PauseInputSource>(true);

            // 按需输入源：由各自系统在需要时启用，此处确保初始为关闭
            SetInputSystemSource<UIReturnInputSource>(false);
            SetInputSystemSource<VHSToggleInputSource>(false);
            SetInputSystemSource<DialogueInputSource>(false);
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
