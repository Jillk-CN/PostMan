using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.InputManagement
{
    public class UIReturnInputSource : MonoBehaviour,IInputSource
    {
        private PlayerInputActions inputActions;
        public bool Enabled =>this.enabled;

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
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(true);
            this.enabled = false; 
            this.inputActions.UI.Return.Enable();
        }

        public void Enable()
        {
            //由于共用了Esc键,需要禁用
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(false);
            this.inputActions.UI.Return.Enable();
            this.enabled = true;
        }
        public bool GetReturn()
        {
            return this.inputActions.UI.Return.WasPressedThisFrame();
        }
    }
}
