using System;
using UnityEngine;

namespace PostMan.InputManagement
{
    public class PlayerInteractInputSource : MonoBehaviour, IInputSource
    {
        private PlayerInputActions inputActions;
        private bool interacting;
        public bool Enabled => this.enabled;

        private void Awake()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction();
            this.interacting = false;
            //只要按下的时候认为是在交互
            inputActions.Player.Interact.performed += (context) =>
            {
                this.interacting = true;
            };
        }
        private void OnEnable()
        {
            Enable();
        }
        private void OnDisable()
        {
            Disable();
        }
        public void RefreshInputActions()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction();
            // 重新注册 performed 回调到新的 inputActions 实例
            this.inputActions.Player.Interact.performed += (context) =>
            {
                this.interacting = true;
            };
            // 若当前处于启用状态，需重新 Enable 以激活新 inputActions 上的 Action
            if (this.enabled)
            {
                Enable();
            }
        }

        private void Update()
        {
            if (inputActions.Player.Interact.WasReleasedThisFrame())
            {
                this.interacting = false;
            }
            
        }
        public void Enable()
        {
            this.inputActions.Player.Interact.Enable();
            this.enabled = true;
        }

        public void Disable()
        {
            this.interacting = false;
            this.inputActions.Player.Interact.Disable();
            this.enabled = false;
        }
        /// <summary>
        /// 获得交互输入
        /// </summary>
        /// <returns>是否按下交互键</returns>
        public bool GetInteract()
        {
            return this.interacting;
        }

    }
}
