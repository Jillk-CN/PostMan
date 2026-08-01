using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PostMan.Common;
using PostMan.InputManagement;
using UnityEngine.Events;

namespace PostMan.UI
{
    /// <summary>
    /// 标题场景主 UI 控制器。
    /// 管理一级主菜单面板，并负责面板之间的路由切换。
    /// </summary>
    public class TitleUIManager : MonoSingleton<TitleUIManager>
    {
        // ─────────────────────────────────────────────
        // 面板引用
        // ─────────────────────────────────────────────

        [Header("面板引用")]
        [Tooltip("主菜单一级面板（初始显示）")]
        [SerializeField] private GameObject mainMenuPanel;

        [Tooltip("语言选择二级面板（初始隐藏）")]
        [SerializeField] private GameObject languagePanelGO;


        // ─────────────────────────────────────────────
        // 主菜单按钮
        // ─────────────────────────────────────────────

        [Header("主菜单按钮")]
        [Tooltip("开始游戏按钮")]
        [SerializeField] private Button btnStartGame;

        [Tooltip("继续游戏按钮（暂留空）")]
        [SerializeField] private Button btnContinue;

        [Tooltip("设置按钮")]
        [SerializeField] private Button btnSettings;

        [Tooltip("切换语言按钮")]
        [SerializeField] private Button btnLanguage;

        [Tooltip("退出游戏按钮")]
        [SerializeField] private Button btnQuit;

        // ─────────────────────────────────────────────
        // 场景地址配置
        // ─────────────────────────────────────────────

        [Header("场景地址配置")]
        [Tooltip("开始游戏时加载的场景 Addressable key 列表")]
        [SerializeField] private List<string> scenesToLoad = new List<string>();

        [Tooltip("开始游戏时卸载的场景 Addressable key 列表")]
        [SerializeField] private List<string> scenesToUnload = new List<string>();

        [Tooltip("开始游戏时玩家的位置")]
        [SerializeField] private Vector3 playerStartPosition;

        public UnityEvent OpenSettingsEvent { get; private set; } = new UnityEvent();
        public UnityEvent CloseSettingsEvent { get; private set; } = new UnityEvent();

        // ─────────────────────────────────────────────
        // Unity 生命周期
        // ─────────────────────────────────────────────

        protected override void Init()
        {
            //DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject); // 标题场景 UI 保留在 DontDestroyOnLoad，避免切换场景时被销毁
            // 初始面板状态
            mainMenuPanel.SetActive(true);
            languagePanelGO.SetActive(false);
            CloseSettingsEvent?.Invoke(); // 初始关闭设置面板事件
            if (TitleUIManager.Instance!=this)
            {
                Destroy(this.gameObject);
            }

            // 标题场景显示鼠标
            GameInputManager.Instance.ShowCursor();

            // 绑定按钮
            btnStartGame.onClick.AddListener(OnStartGame);
            btnSettings.onClick.AddListener(OnOpenSettings);
            btnLanguage.onClick.AddListener(OnOpenLanguage);
            btnQuit.onClick.AddListener(OnQuit);
            // btnContinue 暂不绑定
        }

        private void OnDestroy()
        {
            btnStartGame.onClick.RemoveListener(OnStartGame);
            btnSettings.onClick.RemoveListener(OnOpenSettings);
            btnLanguage.onClick.RemoveListener(OnOpenLanguage);
            btnQuit.onClick.RemoveListener(OnQuit);
        }

        // ─────────────────────────────────────────────
        // 主菜单按钮响应
        // ─────────────────────────────────────────────

        /// <summary>开始游戏：切换至游戏场景。</summary>
        private void OnStartGame()
        {
            BlackScreen.Instance?.BlackIn("", 1f, () =>
            {
                GameSceneManager.Instance.SwitchScenes(scenesToLoad, scenesToUnload, playerStartPosition);
                BlackScreen.Instance?.BlackOut("", 1f);
            });
            GameInputManager.Instance.HideCursor(); // 进入游戏前隐藏鼠标
            HideTitleUI(); // 隐藏标题 UI，避免残留在游戏场景
        }

        /// <summary>开始游戏时隐藏标题 UI（Canvas 保留在 DontDestroyOnLoad，仅隐藏内容）。</summary>
        public void HideTitleUI()
        {
            mainMenuPanel.SetActive(false);
            languagePanelGO.SetActive(false);
            CloseSettingsEvent?.Invoke(); // 关闭设置面板事件
        }

        /// <summary>返回标题时重新显示主菜单面板。</summary>
        public void ShowTitleUI()
        {
            mainMenuPanel.SetActive(true);
            languagePanelGO.SetActive(false);
            CloseSettingsEvent?.Invoke(); // 关闭设置面板事件
        }

        /// <summary>打开设置面板。</summary>
        private void OnOpenSettings()
        {
            mainMenuPanel.SetActive(false);
            OpenSettingsEvent?.Invoke(); // 打开设置面板事件
        }

        /// <summary>打开语言选择面板。</summary>
        private void OnOpenLanguage()
        {
            mainMenuPanel.SetActive(false);
            languagePanelGO.SetActive(true);
        }

        /// <summary>退出游戏。</summary>
        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ─────────────────────────────────────────────
        // 供子面板调用：返回主菜单
        // ─────────────────────────────────────────────

        /// <summary>
        /// 隐藏所有二级面板，返回主菜单。
        /// 由 LanguagePanel 和 SettingsPanel 的返回按钮调用。
        /// </summary>
        public void ReturnToMainMenu()
        {
            languagePanelGO.SetActive(false);
            CloseSettingsEvent?.Invoke(); // 关闭设置面板事件
            mainMenuPanel.SetActive(true);
        }
    }
}
