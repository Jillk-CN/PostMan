using System;
using UnityEngine;

namespace Fountain.UI
{
    [CreateAssetMenu(menuName = "Note")]
    public class NoteContent:ScriptableObject
    {
        [Header("笔记文本")]
        [Header("英文文本")]
        [Tooltip("标题(英文)")]
        public string titleEN;
        [Tooltip("内容(英文)")]
        [TextArea(3, 10)]
        public string textEN;

        [Header("中文文本")]
        [Tooltip("标题(中文)")]
        public string titleZH;
        [Tooltip("内容(中文)")]
        [TextArea(3, 10)]
        public string textZH;
        public string GetTitle()
        {
            return string.Empty;
            /*
            switch (LocalizationManager.Instance.GetLocale())
            {
                case LocalizationManager.LocaleID.zh:
                    return titleZH;
                //break;
                case LocalizationManager.LocaleID.en:
                    return titleEN;
                //break;
                default:
                    return string.Empty;
            }
             */
        }
        public string GetText()
        {
            return string.Empty;
            /*
            switch (LocalizationManager.Instance.GetLocale())
            {
                case LocalizationManager.LocaleID.zh:
                    return textZH;
                //break;
                case LocalizationManager.LocaleID.en:
                    return textEN;
                //break;
                default:
                    return string.Empty;
            }
             */
        }

    }
}
