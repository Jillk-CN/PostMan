using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PostMan.UI
{
    /// <summary>
    /// 渐变效果
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeEffect : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
       // [Tooltip("过渡持续过程,在调用前设置好")]
       // private float duration;
        /*
        [Tooltip("是否默认隐藏")]
        [SerializeField]
        private bool hiddenDefault=true;

         */

        private Coroutine fadeCoroutine;
        private Action fadeCallback;
        private void Awake()
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
        }
        /// <summary>
        /// 淡入
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="fadeEnd">回调</param>
        public void FadeIn(float duration, Action fadeEnd = null)
        {
            if (fadeCoroutine!=null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCallback = fadeEnd;
            fadeCoroutine = StartCoroutine(Fade(0, 1,duration));
        }
        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="fadeEnd">回调</param>
        public void FadeOut(float duration,Action fadeEnd=null)
        {
            if (fadeCoroutine!=null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCallback = fadeEnd;
            fadeCoroutine = StartCoroutine(Fade(1, 0,duration));
        }
        /// <summary>
        /// 过渡的协程
        /// </summary>
        private IEnumerator Fade(float start,float end,float duration)
        {
            canvasGroup.alpha = start;
            float elapsed = 0;
            while (elapsed<duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = end;
            fadeCoroutine = null;
            fadeCallback?.Invoke();
            fadeCallback = null;
        }
    }
}
