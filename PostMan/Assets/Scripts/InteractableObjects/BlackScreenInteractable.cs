using PostMan.InputManagement;
using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class BlackScreenInteractable : MonoBehaviour, IInteractable
    {
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private bool canInteract = true;
        [SerializeField]
        private int priority;

        [Header("黑屏时间设置")]
        [SerializeField]
        private float delayBeforeScreen;
        [SerializeField]
        private float fadeInTime;
        [SerializeField]
        private float blackDuration;
        [SerializeField]
        private float fadeOutTime;
        [SerializeField]
        private string blackText;//到时候要用本地化获取
       
        public int Priority { get => priority; set => priority=value; }

        public void InteractWith(PlayerInteractor player)
        {
            StartCoroutine(DelayBlackScreen());    
        }
        private IEnumerator DelayBlackScreen()
        {
            GameInputManager.Instance.SetPlayerAllInput(false);

            if (delayBeforeScreen!=0)
            {
                yield return new WaitForSeconds(delayBeforeScreen);
            }

            BlackScreen.Instance.BlackInOut(blackText, fadeInTime, blackDuration, fadeOutTime);
            BlackScreen.Instance.BlackInOut(blackText, fadeInTime, blackDuration, fadeOutTime);
            yield return new WaitForSeconds(fadeInTime+blackDuration+fadeOutTime);
            GameInputManager.Instance.SetPlayerAllInput(true);
        }

    }
}
