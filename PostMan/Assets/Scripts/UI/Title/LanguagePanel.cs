using UnityEngine;
using UnityEngine.UI;
using PostMan.Localization;

namespace PostMan.UI
{
    /// <summary>
    /// 语言选择二级面板。
    /// 提供中文 / English 两个按钮切换语言，切换后返回主菜单。
    /// </summary>
    public class LanguagePanel : MonoBehaviour
    {
        // ─────────────────────────────────────────────
        // 按钮引用
        // ─────────────────────────────────────────────

        [Header("语言按钮")]
        [Tooltip("选择简体中文")]
        [SerializeField] private Button btnZhCN;

        [Tooltip("选择 English")]
        [SerializeField] private Button btnEnglish;

        [Tooltip("返回主菜单")]
        [SerializeField] private Button btnBack;

        // ─────────────────────────────────────────────
        // Unity 生命周期
        // ─────────────────────────────────────────────

        private void Awake()
        {
            btnZhCN.onClick.AddListener(OnSelectZhCN);
            btnEnglish.onClick.AddListener(OnSelectEnglish);
            btnBack.onClick.AddListener(OnBack);
        }

        private void OnDestroy()
        {
            btnZhCN.onClick.RemoveListener(OnSelectZhCN);
            btnEnglish.onClick.RemoveListener(OnSelectEnglish);
            btnBack.onClick.RemoveListener(OnBack);
        }

        // ─────────────────────────────────────────────
        // 按钮响应
        // ─────────────────────────────────────────────

        /// <summary>切换到简体中文并返回主菜单。</summary>
        private void OnSelectZhCN()
        {
            LocalizationManager.Instance.SetLocale(LocalizationManager.LocaleID.zh);
            TitleUIManager.Instance.ReturnToMainMenu();
        }

        /// <summary>切换到英文并返回主菜单。</summary>
        private void OnSelectEnglish()
        {
            LocalizationManager.Instance.SetLocale(LocalizationManager.LocaleID.en);
            TitleUIManager.Instance.ReturnToMainMenu();
        }

        /// <summary>返回主菜单（不切换语言）。</summary>
        private void OnBack()
        {
            TitleUIManager.Instance.ReturnToMainMenu();
        }
    }
}
