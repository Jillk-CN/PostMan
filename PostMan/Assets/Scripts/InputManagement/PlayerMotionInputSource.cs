using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace PostMan.InputManagement
{
    /// <summary>
    /// 玩家角色输入提供器,提供移动和交互的输入
    /// </summary>
    public class PlayerMotionInputSource : MonoBehaviour,IInputSource
    {
        private PlayerInputActions inputActions;

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

        public void Enable()
        {
            this.inputActions.Player.Move.Enable();
            //this.inputActions.Player.Crouch.Enable();
            this.inputActions.Player.Run.Enable();
            this.enabled = true;
        }
        public void Disable()
        {
            this.inputActions.Player.Move.Disable();
            //this.inputActions.Player.Crouch.Disable();
            this.inputActions.Player.Run.Disable();
            this.enabled = false;
        }

        /// <summary>
        /// 获得移动输入
        /// </summary>
        /// <returns></returns>
        public Vector3 GetMove()
        {
            Vector2 moveVal = inputActions.Player.Move.ReadValue<Vector2>();
            return new Vector3(moveVal.x, 0, moveVal.y);
        }

        /// <summary>
        /// 获得奔跑输入
        /// </summary>
        /// <returns>是否按下奔跑键</returns>
        public bool GetRun()
        {
            //不知为何,不能直接读取为bool,只能转换下
            return Convert.ToBoolean(inputActions.Player.Run.ReadValue<float>());
        }
    }
}
/*
交互的放在这里 
        /// <summary>
        /// 由于要支持长按的交互,因此用一个变量记录是否“按下交互键”
        /// </summary>
        private bool isInteracting;
 
            //只要按下的时候认为是在交互
            inputActions.Player.Interact.performed += (context) =>
            {
                this.isInteracting = true;
            };
            this.isInteracting = false;

        /// <summary>
        /// 获得交互输入
        /// </summary>
        /// <returns>是否按下交互键</returns>
        public bool GetInteract()
        {
            return this.isInteracting;
        }
        private void Update()
        {
            if (inputActions.Player.Interact.WasReleasedThisFrame())
            {
                this.isInteracting = false;
            }
        }
            this.isInteracting = false;//禁用的时候,要调用这个
 */
