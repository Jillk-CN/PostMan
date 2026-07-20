using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;


namespace PostMan.Localization
{
    /// <summary>
    /// 该类用于本地化多选框,由于UI里的多选框选项的文本内容时动态生成的,
    /// 使用这个类提供的方法来修改多选框里的内容,
    /// 出于一些考量,不用作为装饰类,直接调用这里方法即可
    /// </summary>
    public class LocalizeDropdown : MonoBehaviour
    {
        [Tooltip("多选框选项的内容,手动赋值")]
        [SerializeField]
        private LocalizedString[] options;
        [Tooltip("要本地化的多选框,手动赋值")]
        [SerializeField]
        private TMP_Dropdown dropdown;
        [Tooltip("在启用时自动初始化一次")]
        [SerializeField]
        private bool initOnEnable=false;
        private void Awake()
        {
            LocalizationManager.Instance.LocaleChanged += UpdateOptionText;
        }
        private void OnEnable()
        {
            if (initOnEnable)
            {
                SetOptionText();
                initOnEnable = false;
            }
        }
        /// <summary>
        /// 本地化选项文本,在Editor里配置好文本
        /// </summary>
        public void SetOptionText()
        {
            int previousValue = dropdown.value;
            //修改文本
            List<string> localizedOptions = GetLocalizedOptions();
            dropdown.ClearOptions();
            dropdown.AddOptions(localizedOptions);
            dropdown.RefreshShownValue();

            //保持选项不变
            dropdown.SetValueWithoutNotify(previousValue);
            //dropdown.value = previousValue;
        }
        private List<string> GetLocalizedOptions()
        {
            List<string> localizedOptions = new List<string>();
            for (int i = 0; i < options.Length; i++)
            {
                localizedOptions.Add(options[i].GetLocalizedString());
            }
            return localizedOptions;
        }
        private void UpdateOptionText(LocalizationManager.LocaleID locale)
        {
            SetOptionText();
        }

    }
}
