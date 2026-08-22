using PostMan.InputManagement;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class DisablePlayer : MonoBehaviour
    {
        private PlayerSightInputSource input;
        private void Start()
        {
            input = GameInputManager.Instance.
                GetInputSystemSource<PlayerSightInputSource>();
        }
        private void Update()
        {
            if (input.Enabled)
            {
                GameInputManager.Instance.SetPlayerAllInput(false);
            }    
        }
    }
}
