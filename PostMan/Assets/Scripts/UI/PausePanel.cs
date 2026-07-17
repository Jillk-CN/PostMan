using PostMan.Common;
using PostMan.InputManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace PostMan.UI
{
    /// <summary>
    /// 游戏内暂停面板（单例）。
    /// 按 ESC 呼出/关闭，暂停 timeScale，启用背景虚化 Volume，
    /// 提供"返回游戏"、"设置"、"返回主菜单"三个按钮。
    /// 遵循 ViewImagePanel 的 Show/Hide 模式，Show/Hide 时手动管理输入源。
    /// </summary>
    public class PausePanel : MonoSingleton<PausePanel>
    {
        // ─────────────────────────────────────────────
        // 面板引用
        // ─────────────────────────────────────────────

        [Header("面板内容根节点")]
        [Tooltip("包含所有 UI 子节点的根 GameObject（Show/Hide 控制此节点）")]
        [SerializeField] private GameObject panelContent;

        [Header("面板引用")]
        [Tooltip("游戏内设置面板（共用 SettingsPanel 组件）")]
        [SerializeField] private SettingsPanel settingsPanel;

        // ─────────────────────────────────────────────
        // 场景配置（返回主菜单）
        // ─────────────────────────────────────────────

        [Header("场景配置（返回主菜单）")]
        [Tooltip("返回主菜单时需要加载的场景 Addressable 地址列表")]
        [SerializeField] private List<string> titleScenesToLoad;

        [Tooltip("返回主菜单时需要卸载的场景 Addressable 地址列表")]
        [SerializeField] private List<string> titleScenesToUnload;

        // ─────────────────────────────────────────────
        // 后处理 / 背景虚化
        // ─────────────────────────────────────────────

        [Header("背景虚化")]
        [Tooltip("暂停时启用的 URP Global Volume（含 Depth of Field）")]
        [SerializeField] private Volume pauseVolume;

        // ─────────────────────────────────────────────
        // 按钮
        // ─────────────────────────────────────────────

        [Header("按钮")]
        [Tooltip("返回游戏")]
        [SerializeField] private Button btnResumeGame;

        [Tooltip("打开设置面板")]
        [SerializeField] private Button btnSettings;

        [Tooltip("返回主菜单")]
        [SerializeField] private Button btnReturnToTitle;

        // ─────────────────────────────────────────────
        // 内部状态
        // ─────────────────────────────────────────────

        /// <summary>ESC 输入源，从 GameInputManager 获取。</summary>
        private PauseInputSource pauseInput;

        /// <summary>子面板阻断计数器：大于 0 时 ESC 不触发暂停切换。</summary>
        private int _subPanelBlockCount = 0;

        public UnityEvent OpenSettingsEvent { get; private set; } = new UnityEvent();
        public UnityEvent CloseSettingsEvent { get; private set; } = new UnityEvent();
        public UnityEvent ReturnToTitleEvent { get; private set; } = new UnityEvent();

        // ─────────────────────────────────────────────
        // Unity 生命周期
        // ─────────────────────────────────────────────

        private void Awake()
        {
            pauseInput = GameInputManager.Instance.GetInputSystemSource<PauseInputSource>();

            btnResumeGame.onClick.AddListener(OnResumeGame);
            btnSettings.onClick.AddListener(OnOpenSettings);
            btnReturnToTitle.onClick.AddListener(OnReturnToTitle);

            // 初始隐藏
            panelContent.SetActive(false);
        }

        private void OnDestroy()
        {
            btnResumeGame.onClick.RemoveListener(OnResumeGame);
            btnSettings.onClick.RemoveListener(OnOpenSettings);
            btnReturnToTitle.onClick.RemoveListener(OnReturnToTitle);
        }

        private void Update()
        {
            if (pauseInput == null || !pauseInput.Enabled) return;
            if (_subPanelBlockCount > 0) return; // 子面板打开时屏蔽 ESC 切换暂停

            if (pauseInput.GetPause())
            {
                if (panelContent.activeSelf)
                    Hide();
                else
                    Show();
            }
        }

        // ─────────────────────────────────────────────
        // 公开接口
        // ─────────────────────────────────────────────

        /// <summary>
        /// 显示暂停面板：冻结时间，启用虚化，禁用玩家输入，显示光标。
        /// </summary>
        public void Show()
        {
            panelContent.SetActive(true);
            Time.timeScale = 0f;

            if (pauseVolume != null)
                pauseVolume.enabled = true;

            GameInputManager.Instance.SetPlayerAllInput(false);
            GameInputManager.Instance.ShowCursor();
            // 禁用 UIReturn，避免与 ESC（Pause）键冲突
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(false);
        }

        /// <summary>
        /// 隐藏暂停面板：恢复时间，关闭虚化，恢复玩家输入，隐藏光标。
        /// </summary>
        public void Hide()
        {
            panelContent.SetActive(false);
            Time.timeScale = 1f;

            if (pauseVolume != null)
                pauseVolume.enabled = false;

            GameInputManager.Instance.SetPlayerAllInput(true);
            GameInputManager.Instance.HideCursor();
        }

        /// <summary>子面板打开时调用，阻止 ESC 切换暂停。</summary>
        public void BlockEscToggle() => _subPanelBlockCount++;

        /// <summary>子面板关闭时调用，恢复 ESC 切换暂停。</summary>
        public void UnblockEscToggle() => _subPanelBlockCount = Mathf.Max(0, _subPanelBlockCount - 1);

        // ─────────────────────────────────────────────
        // 按钮回调
        // ─────────────────────────────────────────────

        /// <summary>返回游戏：关闭暂停面板。</summary>
        private void OnResumeGame()
        {
            Hide();
        }

        /// <summary>
        /// 打开设置面板。
        /// 向 SettingsPanel.onBack 注入"返回暂停面板"的回调，
        /// 然后隐藏本面板，显示设置面板。
        /// </summary>
        private void OnOpenSettings()
        {
            if (settingsPanel == null)
            {
                //Debug.LogWarning("[PausePanel] settingsPanel 未赋值，请在 Inspector 中配置。");
                //return;
            }

            // 注入回调：设置面板返回时重新显示暂停面板
            settingsPanel.onBack = OnSettingsPanelBack;

            BlockEscToggle(); // 设置面板打开时屏蔽 ESC
            panelContent.SetActive(false);
            OpenSettingsEvent.Invoke(); // 触发外部事件，通知 SettingsPanel 显示
        }

        /// <summary>从设置面板返回时重新显示暂停面板。</summary>
        private void OnSettingsPanelBack()
        {
            settingsPanel.onBack = null; // 清理回调，避免下次从标题打开设置时误触发
            CloseSettingsEvent.Invoke(); // 触发外部事件，通知 SettingsPanel 隐藏
            panelContent.SetActive(true);
            UnblockEscToggle(); // 设置面板关闭，恢复 ESC
        }

        /// <summary>
        /// 返回主菜单：恢复 timeScale 和光标，然后执行场景切换。
        /// </summary>
        private void OnReturnToTitle()
        {
            Time.timeScale = 1f;
            GameInputManager.Instance.ShowCursor();

            if (pauseVolume != null)
                pauseVolume.enabled = false;

            TitleUIManager.Instance?.ShowTitleUI(); // 场景切换前恢复标题 UI
            ReturnToTitleEvent.Invoke();
            GameSceneManager.Instance.SwitchScenes(titleScenesToLoad, titleScenesToUnload);
        }
    }
}
