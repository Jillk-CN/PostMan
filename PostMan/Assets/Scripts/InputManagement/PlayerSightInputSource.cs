using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.InputManagement
{
    /// <summary>
    /// 玩家视线输入提供器,提供与相机相关的输入
    /// </summary>
    public class PlayerSightInputSource : MonoBehaviour,IInputSource
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
            this.inputActions.Player.Look.Enable();
            this.enabled = true;
        }

        public void Disable()
        {
            this.inputActions.Player.Look.Disable();
            this.enabled = false;
        }

        /// <summary>
        /// 获得视线移动的输入
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 GetSightMove()
        {
            return inputActions.Player.Look.ReadValue<Vector2>();
        }

    }
}
