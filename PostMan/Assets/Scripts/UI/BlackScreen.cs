using PostMan.Common;
using PostMan.InputManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PostMan.UI
{
    /// <summary>
    /// 全局的黑屏效果
    /// </summary>
    [RequireComponent(typeof(FadeEffect))]
    public class BlackScreen :MonoSingleton<BlackScreen>
    {
        private FadeEffect fadeEffect;
        private TextMeshProUGUI blackText;

        //[Header("黑屏需要的数据")]
        //public float fadeInTime;
        //public float fadeOutTime;
        //public float duration;
        //private float elapsed;

        protected override void Init()
        {
            base.Init();
            fadeEffect = this.GetComponent<FadeEffect>();
            blackText = this.transform.FindChildByName(nameof(blackText)).
                GetComponent<TextMeshProUGUI>();
        }
        /// <summary>
        /// 淡入淡出
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fadeInTime"></param>
        /// <param name="duration"></param>
        /// <param name="fadeOutTime"></param>
        public void BlackInOut(string text="",
            float fadeInTime=0.1f,
            float duration=0.5f,
            float fadeOutTime=0.1f)
        {
            fadeEffect.FadeIn(fadeInTime, 
                () => { StartCoroutine(DelayFadeOut(duration, fadeOutTime)); }
                );
            blackText.text = text;
        }
        /// <summary>
        /// 仅淡出
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fadeOutTime"></param>
        public void BlackOut(string text="",
            float fadeOutTime=0.1f)
        {
            fadeEffect.FadeOut(fadeOutTime);
            blackText.text = text;
        }

        /// <summary>
        /// 仅淡入
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fadeInTime"></param>
        public void BlackIn(string text="",
            float fadeInTime=0.1f)
        {
            fadeEffect.FadeIn(fadeInTime);
            blackText.text = text;
        }

        private IEnumerator DelayFadeOut(float delay,float fadeOutTime)
        {
            yield return new WaitForSeconds(delay);
            fadeEffect.FadeOut(fadeOutTime);
        }
    }
}
