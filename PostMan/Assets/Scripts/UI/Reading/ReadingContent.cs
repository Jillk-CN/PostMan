using PostMan.Localization;
using System;
using UnityEngine;

namespace PostMan.UI
{
    [CreateAssetMenu(menuName = "ReadingSO")]
    public class ReadingContent:ScriptableObject
    {
        [Tooltip("本地化的键")]
        [SerializeField]
        private string key;

        /*
        private string text;
        public string GetTitle()
        {
            return "NOT FOUND";
        }
         */

        public string GetText()
        {
            return LocalizationManager.Instance.
                GetLocalizedString(LocalizationManager.TableName.ReadingMaterials,key);
        }

    }
}
