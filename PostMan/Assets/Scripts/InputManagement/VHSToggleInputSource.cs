using PostMan.UI;
using UnityEngine;

namespace PostMan.InputManagement
{
    /// <summary>
    /// 封装 ToggleVHS 输入动作（O 键），用于切换 VHS 调节面板的显隐。
    /// 挂载到与 GameInputManager 相同的 GameObject 上，始终活跃，
    /// 因此可以在面板隐藏后继续检测输入。
    /// </summary>
    public class VHSToggleInputSource : MonoBehaviour, IInputSource
    {
        private PlayerInputActions inputActions;

        /// <summary>当前是否启用</summary>
        public bool Enabled => this.enabled;

        private void Awake()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction();
        }

        private void OnEnable()
        {
            Enable();
        }

        private void OnDisable()
        {
            Disable();
        }

        // 在始终活跃的 GameObject 上轮询，避免面板隐藏后 Update 失效
        private void Update()
        {
            if (GetToggle())
            {
                VHSPanel.Instance.Toggle();
            }
        }

        /// <summary>禁用 ToggleVHS 动作</summary>
        public void Disable()
        {
            this.inputActions.UI.ToggleVHS.Disable();
            this.enabled = false;
        }

        /// <summary>启用 ToggleVHS 动作</summary>
        public void Enable()
        {
            this.inputActions.UI.ToggleVHS.Enable();
            this.enabled = true;
        }

        public void RefreshInputActions()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction();
            // 若当前处于启用状态，需重新 Enable 以激活新 inputActions 上的 Action
            if (this.enabled)
            {
                Enable();
            }
        }

        /// <summary>本帧是否按下了 O 键</summary>
        public bool GetToggle()
        {
            return this.inputActions.UI.ToggleVHS.WasPressedThisFrame();
        }
    }
}
