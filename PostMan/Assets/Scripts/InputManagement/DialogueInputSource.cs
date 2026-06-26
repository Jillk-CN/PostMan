using PostMan.InputManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class DialogueInputSource : MonoBehaviour, IInputSource
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
            this.inputActions.UI.ContinueDialogue.Disable();
            this.enabled = false; 
        }

        public void Enable()
        {
            //都用了同一个Esc键
            this.inputActions.UI.ContinueDialogue.Enable();
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

        public bool GetContinue()
        {
            return this.inputActions.UI.ContinueDialogue.WasPressedThisFrame();
        }
    }
}
