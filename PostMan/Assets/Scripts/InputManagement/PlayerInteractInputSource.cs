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
