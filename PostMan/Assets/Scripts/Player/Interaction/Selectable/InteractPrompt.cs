using PostMan.Common;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace PostMan.Player
{
    /// <summary>
    /// 玩家选中物体时显示的提示
    /// </summary>
    public class InteractPrompt :MonoSingleton<InteractPrompt>//非常不情愿地做成单例
    {
        // 准星中心偏移，保证不遮挡物体
        // [Tooltip("准星偏移量，用于将提示显示在偏离中心的位置")]
        // public Vector2 screenOffset;
        private CanvasGroup canvasGroup;
        private bool transitioning;
        private float startAlpha;
        private float targetAlpha;
        [SerializeField]
        private float duration;
        private float elapsed;
        protected override void Init()
        {
            base.Init();
            canvasGroup = this.GetComponent<CanvasGroup>();
            transitioning = false;
        }
        private void Start()
        {
            //SetOffset();

        }
        private void Update()
        {
            if (!transitioning)
            {
                return;
            }
            if ((elapsed+=Time.unscaledDeltaTime)<duration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            }
            else
            {
                canvasGroup.alpha = targetAlpha;
                this.transitioning = false;
            }
        }
        /*
        private void SetOffset()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            this.transform.position = screenCenter + screenOffset;

        }
         */
        public void Show()
        {
            elapsed = 0;
            this.transitioning = true;
            startAlpha = this.canvasGroup.alpha;
            targetAlpha = 1;

        }
        public void Hide()
        {
            elapsed = 0;
            this.transitioning = true;
            startAlpha = this.canvasGroup.alpha;
            targetAlpha = 0;
        }
    }
}
