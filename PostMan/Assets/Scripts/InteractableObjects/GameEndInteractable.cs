using PostMan.Common;
using PostMan.InputManagement;
using PostMan.Player;
using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameEndInteractable : MonoBehaviour,IInteractable
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

    public string[] load;
    public string[] unload;
        public int Priority { get => priority; set => priority=value; }

        public void InteractWith(PlayerInteractor player)
        {
        GameInputManager.Instance.SetPlayerAllInput(false);
        GameInputManager.Instance.ShowCursor();
        BlackScreen.Instance.BlackIn
            (blackText, fadeInTime);
        Button btn = BlackScreen.Instance.transform.FindChildByName("GameEndButton").
            GetComponent<Button>();
        CanvasGroup grp = btn.transform.parent.GetComponent<CanvasGroup>();
        grp.interactable = true;
        grp.blocksRaycasts = true;
        btn.gameObject.SetActive(true); 
        btn.onClick.AddListener ( () =>
        {
            GameSceneManager.Instance.SwitchScenes(load.ToList(), unload.ToList(),
                Vector2.zero);
            PausePanel.Instance.Publish();
            grp.interactable = false;
            grp.blocksRaycasts = false;
        });
        }
}
