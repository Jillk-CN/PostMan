using PostMan.InputManagement;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    public class MoveVcamTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool triggerOnce=true;
        [SerializeField]
        private bool canTrigger=true;
        [SerializeField]
        private MoveVcamInPath moveVcam;
        [SerializeField]
        private float backDelay=1f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")&&canTrigger)
            {
                GameInputManager.Instance.SetPlayerAllInput(false);
                //一个来回
                moveVcam.MoveVcam(() =>
                {
                    StartCoroutine(DelayMoveBack());
                });

                if (triggerOnce) 
                {
                    this.canTrigger = false;
                } 
            }
        }
        private IEnumerator DelayMoveBack()
        {
            yield return new WaitForSeconds(backDelay);
            float swapTemp = moveVcam.startPosition;
            moveVcam.startPosition = moveVcam.endPosition;
            moveVcam.endPosition = swapTemp;
            moveVcam.MoveVcam(() =>
            {
                GameInputManager.Instance.SetPlayerAllInput(true);
                moveVcam.vcam.gameObject.SetActive(false);
            });
        }
    }
}
