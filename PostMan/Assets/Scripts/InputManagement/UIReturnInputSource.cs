using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.InputManagement
{
    public class UIReturnInputSource : MonoBehaviour,IInputSource
    {
        private PlayerInputActions inputActions;
        public bool Enabled =>this.enabled;

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
        public void Disable()
        {
            this.enabled = false; 
            this.inputActions.UI.Return.Enable();
        }

        public void Enable()
        {
            //由于共用了Esc键,需要禁用
            this.inputActions.UI.Return.Enable();
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

        public bool GetReturn()
        {
            return this.inputActions.UI.Return.WasPressedThisFrame();
        }
    }
}
