using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.InputManagement
{
    public class PauseInputSource : MonoBehaviour, IInputSource
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
            this.inputActions.UI.Pause.Disable();
            this.enabled = false; 
        }

        public void Enable()
        {
            //都用了同一个Esc键
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(false);
            this.inputActions.UI.Pause.Enable();
            this.enabled = true;
        }

        public bool GetPause()
        {
            return this.inputActions.UI.Pause.WasPressedThisFrame();            
        }
    }
}
