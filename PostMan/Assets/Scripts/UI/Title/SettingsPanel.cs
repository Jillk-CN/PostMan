using PostMan.AudioSystem;
using PostMan.InputManagement;
using PostMan.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PostMan.UI
{
    /// <summary>
    /// 设置二级面板。
    /// 涵盖玩家输入、VHS滤镜、音量（留空）、画面等设置。
    /// 标题场景无玩家，灵敏度和抖动开关写入 PlayerPrefs，游戏场景 PlayerSight.Start() 读取应用。
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        // ─────────────────────────────────────────────
        // PlayerPrefs 键名常量
        // ─────────────────────────────────────────────

        /// <summary>鼠标灵敏度键名，与 PlayerSight.Start() 保持一致。</summary>
        private const string KeySensitivity = "MouseSensitivity";

        /// <summary>视角抖动键名，与 PlayerSight.Start() 保持一致。</summary>
        private const string KeyEnableShake = "EnableShake";

        // ─────────────────────────────────────────────
        // 玩家输入设置
        // ─────────────────────────────────────────────

        [Header("玩家输入设置")]
        [Tooltip("鼠标灵敏度 Slider（建议值域 0.1 ~ 3.0）")]
        [SerializeField] private Slider sliderSensitivity;

        [Tooltip("视角抖动开关 Toggle")]
        [SerializeField] private Toggle toggleEnableShake;

        // ─────────────────────────────────────────────
        // VHS 滤镜
        // ─────────────────────────────────────────────

        [Header("VHS 滤镜")]
        [Tooltip("打开 VHS 滤镜面板按钮")]
        [SerializeField] private Button btnOpenVHS;

        // ─────────────────────────────────────────────
        // 音量设置
        // ─────────────────────────────────────────────

        [Header("音量设置")]
        [Tooltip("总音量 Slider")]
        [SerializeField] private Slider sliderMasterVolume;

        [Tooltip("音乐音量 Slider")]
        [SerializeField] private Slider sliderMusicVolume;

        [Tooltip("音效音量 Slider")]
        [SerializeField] private Slider sliderSFXVolume;

        // ─────────────────────────────────────────────
        // 画面设置
        // ─────────────────────────────────────────────

        [Header("画面设置")]
        [Tooltip("帧率限制 Dropdown（顺序：30/60/120/144/无限制）")]
        [SerializeField] private TMP_Dropdown dropdownFrameRate;

        [Tooltip("分辨率 Dropdown（顺序：720p/1080p/2K）")]
        [SerializeField] private TMP_Dropdown dropdownResolution;

        [Tooltip("窗口模式 Dropdown（顺序：无边框/有边框/全屏）")]
        [SerializeField] private TMP_Dropdown dropdownWindowMode;

        [Tooltip("画面伽马值 Slider（建议值域 0.5 ~ 2.0）")]
        [SerializeField] private Slider sliderGamma;

        // ─────────────────────────────────────────────
        // 导航
        // ─────────────────────────────────────────────

        [Header("导航")]
        [Tooltip("返回主菜单按钮")]
        [SerializeField] private Button btnBack;

        /// <summary>
        /// 返回按钮的自定义回调。
        /// 注入此委托后，OnBack() 将调用它而非默认的 TitleUIManager.ReturnToMainMenu()。
        /// 游戏内使用时由 PausePanel 注入"返回暂停面板"逻辑；标题场景不注入，走默认行为。
        /// </summary>
        public System.Action onBack;

        // ─────────────────────────────────────────────
        // 枚举映射表（与 Dropdown 选项顺序严格对应）
        // ─────────────────────────────────────────────

        /// <summary>帧率选项：-1 表示无限制。</summary>
        private static readonly int[] FrameRateOptions = { 30, 60, 120, 144, -1 };

        /// <summary>分辨率选项（宽, 高）。</summary>
        private static readonly (int w, int h)[] ResolutionOptions =
        {
            (1280,  720),
            (1920, 1080),
            (2560, 1440),
        };

        // ─────────────────────────────────────────────
        // Unity 生命周期
        // ─────────────────────────────────────────────

        private void Awake()
        {
            BindButtons();
            BindControls();
            // 整个生命周期只订阅一次，避免 SetActive(false) 触发 OnDisable 意外取消订阅
            if (VHSPanel.Instance != null)
                VHSPanel.Instance.OnHide += OnVHSPanelClosed;
        }

        private void OnEnable()
        {
            // 面板激活时从持久化状态同步 UI，避免显示过时数据
            SyncUIFromPrefs();
        }

        private void OnDisable() { }

        private void OnDestroy()
        {
            UnbindButtons();
            UnbindControls();
            if (VHSPanel.Instance != null)
                VHSPanel.Instance.OnHide -= OnVHSPanelClosed;
        }

        // ─────────────────────────────────────────────
        // 绑定 / 解绑
        // ─────────────────────────────────────────────

        private void BindButtons()
        {
            btnBack.onClick.AddListener(OnBack);
            btnOpenVHS.onClick.AddListener(OnOpenVHS);
        }

        private void UnbindButtons()
        {
            btnBack.onClick.RemoveListener(OnBack);
            btnOpenVHS.onClick.RemoveListener(OnOpenVHS);
        }

        private void BindControls()
        {
            sliderSensitivity.onValueChanged.AddListener(OnSensitivityChanged);
            toggleEnableShake.onValueChanged.AddListener(OnEnableShakeChanged);
            dropdownFrameRate.onValueChanged.AddListener(OnFrameRateChanged);
            dropdownResolution.onValueChanged.AddListener(OnResolutionChanged);
            dropdownWindowMode.onValueChanged.AddListener(OnWindowModeChanged);
            sliderGamma.onValueChanged.AddListener(OnGammaChanged);
            sliderMasterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
            sliderMusicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);
            sliderSFXVolume.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        private void UnbindControls()
        {
            sliderSensitivity.onValueChanged.RemoveListener(OnSensitivityChanged);
            toggleEnableShake.onValueChanged.RemoveListener(OnEnableShakeChanged);
            dropdownFrameRate.onValueChanged.RemoveListener(OnFrameRateChanged);
            dropdownResolution.onValueChanged.RemoveListener(OnResolutionChanged);
            dropdownWindowMode.onValueChanged.RemoveListener(OnWindowModeChanged);
            sliderGamma.onValueChanged.RemoveListener(OnGammaChanged);
            sliderMasterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            sliderMusicVolume.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            sliderSFXVolume.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }

        // ─────────────────────────────────────────────
        // UI 同步
        // ─────────────────────────────────────────────

        /// <summary>
        /// 从 PlayerPrefs 和系统状态读取并同步所有控件显示值。
        /// 使用 SetValueWithoutNotify / SetIsOnWithoutNotify 避免触发回调。
        /// </summary>
        private void SyncUIFromPrefs()
        {
            sliderSensitivity.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(KeySensitivity, 1f));

            toggleEnableShake.SetIsOnWithoutNotify(
                PlayerPrefs.GetInt(KeyEnableShake, 1) == 1);

            dropdownFrameRate.SetValueWithoutNotify(
                GetFrameRateIndex(Application.targetFrameRate));

            dropdownResolution.SetValueWithoutNotify(
                GetResolutionIndex(Screen.width, Screen.height));

            dropdownWindowMode.SetValueWithoutNotify(
                GetWindowModeIndex(Screen.fullScreenMode));

            sliderGamma.SetValueWithoutNotify(
                PlayerPrefs.GetFloat("Gamma", 1f));

            sliderMasterVolume.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(AudioManager.KeyMasterVolume, 1f));
            sliderMusicVolume.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(AudioManager.KeyBGMVolume, 1f));
            sliderSFXVolume.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(AudioManager.KeySFXVolume, 1f));
        }

        // ─────────────────────────────────────────────
        // 控件响应：玩家输入
        // ─────────────────────────────────────────────

        private void OnSensitivityChanged(float value)
        {
            PlayerPrefs.SetFloat(KeySensitivity, value);
            PlayerPrefs.Save();
            // 游戏场景内实时推送；标题场景无 PlayerSight，FindObjectOfType 返回 null 安全跳过
            var sight = FindObjectOfType<PlayerSight>();
            if (sight != null) sight.sensitivity = value;
        }

        private void OnEnableShakeChanged(bool value)
        {
            PlayerPrefs.SetInt(KeyEnableShake, value ? 1 : 0);
            PlayerPrefs.Save();
            // 游戏场景内实时推送；标题场景无 PlayerSight，FindObjectOfType 返回 null 安全跳过
            var sight = FindObjectOfType<PlayerSight>();
            if (sight != null)
            {
                if (value) sight.EnableShake();
                else sight.DisableShake();
            }
        }

        // ─────────────────────────────────────────────
        // 控件响应：画面设置
        // ─────────────────────────────────────────────

        private void OnFrameRateChanged(int index)
        {
            if (index < 0 || index >= FrameRateOptions.Length) return;
            Application.targetFrameRate = FrameRateOptions[index];
        }

        private void OnResolutionChanged(int index)
        {
            if (index < 0 || index >= ResolutionOptions.Length) return;
            (int w, int h) = ResolutionOptions[index];
            Screen.SetResolution(w, h, Screen.fullScreenMode);
        }

        /// <summary>
        /// Dropdown 顺序：0=无边框(FullScreenWindow)，1=有边框(Windowed)，2=全屏(ExclusiveFullScreen)
        /// </summary>
        private void OnWindowModeChanged(int index)
        {
            switch (index)
            {
                case 0: Screen.fullScreenMode = FullScreenMode.FullScreenWindow;    break;
                case 1: Screen.fullScreenMode = FullScreenMode.Windowed;            break;
                case 2: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
                default:
                    Debug.LogWarning($"[SettingsPanel] 未知窗口模式 index: {index}");
                    break;
            }
        }

        private void OnGammaChanged(float value)
        {
            PlayerPrefs.SetFloat("Gamma", value);
            PlayerPrefs.Save();
            // TODO：将 gamma 推送给 URP Volume 或 Post-processing 组件
        }

        // ─────────────────────────────────────────────
        // 控件响应：音量设置
        // ─────────────────────────────────────────────

        private void OnMasterVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMasterVolume(value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance?.SetBGMVolume(value);
        }

        private void OnSFXVolumeChanged(float value)
        {
            AudioManager.Instance?.SetSFXVolume(value);
        }

        // ─────────────────────────────────────────────
        // 控件响应：VHS / 导航
        // ─────────────────────────────────────────────

        /// <summary>
        /// 打开 VHS 滤镜三级面板。
        /// 先隐藏本面板（SetActive false），再显示 VHSPanel，避免两面板叠层。
        /// VHSPanel.Hide() 触发 OnHide → OnVHSPanelClosed() 重新显示本面板。
        /// </summary>
        private void OnOpenVHS()
        {
            if (VHSPanel.Instance == null)
            {
                Debug.LogWarning("[SettingsPanel] VHSPanel 实例不存在，请在场景中放置 VHSPanel 预制体。");
                return;
            }
            PausePanel.Instance?.BlockEscToggle(); // 游戏场景：VHSPanel 打开时屏蔽 ESC（标题场景无 PausePanel，安全跳过）
            this.gameObject.SetActive(false);   // 隐藏 SettingsPanel
            VHSPanel.Instance.Show();           // 显示 VHSPanel
        }

        /// <summary>VHSPanel 关闭后重新激活本面板，使用户可以继续调整其他设置。</summary>
        private void OnVHSPanelClosed()
        {
            this.gameObject.SetActive(true);
            PausePanel.Instance?.UnblockEscToggle(); // 游戏场景：VHSPanel 关闭，恢复 ESC
            GameInputManager.Instance.ShowCursor();   // 恢复标题场景的鼠标显示状态
        }

        private void OnBack()
        {
            if (onBack != null) onBack.Invoke();
            else TitleUIManager.Instance.ReturnToMainMenu();
        }

        // ─────────────────────────────────────────────
        // 辅助：Dropdown index 映射
        // ─────────────────────────────────────────────

        private static int GetFrameRateIndex(int fps)
        {
            for (int i = 0; i < FrameRateOptions.Length; i++)
                if (FrameRateOptions[i] == fps) return i;
            return FrameRateOptions.Length - 1; // 默认"无限制"
        }

        private static int GetResolutionIndex(int w, int h)
        {
            for (int i = 0; i < ResolutionOptions.Length; i++)
                if (ResolutionOptions[i].w == w && ResolutionOptions[i].h == h) return i;
            return 1; // 默认 1080p
        }

        private static int GetWindowModeIndex(FullScreenMode mode)
        {
            return mode switch
            {
                FullScreenMode.FullScreenWindow    => 0,
                FullScreenMode.Windowed            => 1,
                FullScreenMode.ExclusiveFullScreen => 2,
                _                                  => 0,
            };
        }
    }
}
