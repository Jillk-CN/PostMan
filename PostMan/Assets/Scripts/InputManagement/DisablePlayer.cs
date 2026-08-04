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
            StartCoroutine(DisablePlayerNextFrame());
        }
        private IEnumerator DisablePlayerNextFrame()
        {
            yield return null;
            GameInputManager.Instance.SetPlayerAllInput(false);

        }
    }
}
