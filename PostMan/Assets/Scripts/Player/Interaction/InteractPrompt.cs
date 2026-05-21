using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.UI
{
    /// <summary>
    /// 玩家选中物体时显示的提示
    /// </summary>
    public class InteractPrompt : MonoBehaviour
    {
        // 准星中心偏移，保证不遮挡物体
        [Tooltip("准星偏移量，用于将提示显示在偏离中心的位置")]
        public Vector2 screenOffset;

        private void Start()
        {
            PlayerInteractor playerInteractor = PlayerInstance.Instance.
                GetComponent<PlayerInteractor>();
           // playerInteractor.SelectInteractable += Show;
           // playerInteractor.DeselectInteractable += Hide;
            Hide();
            SetOffset();
        }
        private void SetOffset()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            this.transform.position = screenCenter + screenOffset;

        }
        private void Show()
        {
            this.gameObject.SetActive(true);
        }
        private void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
