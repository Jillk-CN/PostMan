using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

namespace PostMan.UI
{
    /// <summary>
    /// 实现打字机效果的类,不支持富文本
    /// </summary>
    public class TypeEffect
    {
        /// <summary>
        /// 要显示的文本组件
        /// </summary>
        private TMP_Text displayText;
        /// <summary>
        /// 要显示的完整文本
        /// </summary>
        private string fullText;
        /// <summary>
        /// 字符串显示的速度,字符/秒
        /// </summary>
        private float typeSpeed=25;//25即1/25秒显示一个字符,用这个值作为协程的间隔
        /// <summary>
        /// 是否在打字中
        /// </summary>
        private bool typing;
        /// <summary>
        /// 打字机效果的协程
        /// </summary>
        private Coroutine typingEffectCoroutine;
        /// <summary>
        /// 打字机效果完成
        /// </summary>
        public event Action TypeCompleted;

        public TypeEffect(TMP_Text displayText,float typeSpeed)
        {
            this.displayText = displayText;
            this.SetTypeSpeed(typeSpeed);
            typing = false;
        }


        /// <summary>
        /// 用打字机的效果显示文字
        /// </summary>
        public void ShowText(string text)
        {
            if (typing)
            {
                return;
            }
            this.fullText = text;
            typingEffectCoroutine = displayText.StartCoroutine(this.ApplyEffect()); 
        }
        /// <summary>
        /// 直接显示,不要在打字时调用该方法
        /// </summary>
        public void ShowTextDirectly(string text)
        {
            if (typing)
            {
                return;
            }
            displayText.text = text;     
            TypeCompleted?.Invoke();
        }
        /// <summary>
        /// 跳过打字机的效果,直接将全部文本显示
        /// </summary>
        public void SkipEffect()
        {
            if (!typing)
            {
                return;
            }
            displayText.StopCoroutine(typingEffectCoroutine);
            typing = false;
            displayText.text = fullText;
            TypeCompleted?.Invoke();
        }

        public bool IsTyping()
        {
            return this.typing;
        }

        /// <summary>
        /// 设置打字机速度,不能小于等于0
        /// </summary>
        /// <param name="typeSpeed"></param>
        public void SetTypeSpeed(float typeSpeed)
        {
            this.typeSpeed = Math.Clamp(typeSpeed, 0, float.MaxValue);
        }


        /// <summary>
        /// 打字机效果的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator ApplyEffect()
        {
            this.typing = true;
            //简单起见,用一个字一个字的追加来模拟打字机效果
            StringBuilder builder = new StringBuilder();
            displayText.text = string.Empty;
            //返回值是IEnumerator,不能用foreach
            var enumerator = StringInfo.GetTextElementEnumerator(fullText);
            //和foreach一样调用MoveNext(),只不过不用Current(在官方文档里的示例代码没看到用Current),
            //用GetTextElement获取遍历到的元素
            while (enumerator.MoveNext())
            {
                builder.Append(enumerator.GetTextElement());
                displayText.text = builder.ToString();
                yield return new WaitForSeconds(1 / typeSpeed);
            }
            typing = false;
            TypeCompleted?.Invoke();
        }
    }
}
