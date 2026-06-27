using PostMan.InputManagement;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class DisablePlayer : MonoBehaviour
    {
        private void Start()
        {
            GameInputManager.Instance.SetPlayerAllInput(false);
        }
    }
}
