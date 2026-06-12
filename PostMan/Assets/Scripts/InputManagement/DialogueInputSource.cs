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
        public bool GetContinue()
        {
            return this.inputActions.UI.ContinueDialogue.WasPressedThisFrame();
        }
    }
}
