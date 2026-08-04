using PostMan.InputManagement;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    /// <summary>
    /// 强制玩家看向某物若干时间
    /// </summary>
    public class ForceLookAtTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool triggerOnce=true;
        [SerializeField]
        private bool canTrigger=true;
        [SerializeField]
        private Transform target;
        [SerializeField]
        private float lookDuration = 1f;
        [SerializeField]
        private float enableInputDelay = 3f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")&&canTrigger)
            {
                PlayerMotion motion = other.GetComponent<PlayerMotion>();
                if (motion==null)
                {
                    Debug.Log("Motion Not Found");
                    return;
                }
                StartCoroutine(ForceLookAt(motion));
                if (triggerOnce) 
                {
                    this.canTrigger = false;
                } 
            }
        }
        private IEnumerator ForceLookAt(PlayerMotion motion)
        {
            GameInputManager.Instance.SetPlayerAllInput(false);
                motion.LookAt(target.position, lookDuration);
            yield return new WaitForSeconds(enableInputDelay);
            GameInputManager.Instance.SetPlayerAllInput(true);
        }

    }
}
