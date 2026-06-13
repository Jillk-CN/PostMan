using System.Collections;
using PostMan.Localization;
using TMPro;
using UnityEngine;

/// <summary>
/// 任务 UI 控制器。
/// 订阅 TaskEventBus 事件，在任务启动/推进/完成时更新显示内容。
/// 任务触发时面板从屏幕左侧滑入，完成后原路滑出。
/// 按 Tab 键可随时切换面板的显示/隐藏状态。
/// </summary>
public class TaskUI : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // UI 引用
    // ─────────────────────────────────────────────

    [Header("UI 引用")]

    /// <summary>任务面板的 RectTransform，用于控制滑入/滑出动画。</summary>
    [Tooltip("任务面板的 RectTransform，用于控制滑入/滑出动画")]
    [SerializeField] private RectTransform taskPanel;

    /// <summary>显示任务名称的 TMP 文本组件。</summary>
    [Tooltip("显示任务名称的 TMP 文本组件")]
    [SerializeField] private TMP_Text nameText;

    /// <summary>显示任务描述的 TMP 文本组件。</summary>
    [Tooltip("显示任务描述的 TMP 文本组件")]
    [SerializeField] private TMP_Text descText;

    /// <summary>显示任务进度的 TMP 文本组件（格式：当前/总计）。</summary>
    [Tooltip("显示任务进度的 TMP 文本组件（格式：当前/总计）")]
    [SerializeField] private TMP_Text progressText;

    // ─────────────────────────────────────────────
    // 动画设置
    // ─────────────────────────────────────────────

    [Header("动画设置")]

    /// <summary>面板滑入/滑出动画的持续时间（秒）。</summary>
    [Tooltip("面板滑入/滑出动画的持续时间（秒）")]
    [SerializeField] private float slideDuration = 0.4f;

    // ─────────────────────────────────────────────
    // 私有状态
    // ─────────────────────────────────────────────

    private bool _isVisible;          // 面板当前是否可见
    private Coroutine _slideCoroutine; // 当前正在运行的滑动协程
    private TaskRuntimeData _currentData; // 缓存当前任务，供语言切换时刷新

    // ─────────────────────────────────────────────
    // Unity 生命周期
    // ─────────────────────────────────────────────

    private void Awake()
    {
        // 初始状态：面板隐藏在屏幕左侧
        if (taskPanel != null)
            taskPanel.anchoredPosition = new Vector2(-taskPanel.rect.width, taskPanel.anchoredPosition.y);
    }

    private void OnEnable()
    {
        // 订阅事件总线
        TaskEventBus.OnTaskStarted += HandleTaskStarted;
        TaskEventBus.OnTaskAdvanced += HandleTaskAdvanced;
        TaskEventBus.OnTaskCompleted += HandleTaskCompleted;
        TaskEventBus.OnTaskFailed += HandleTaskFailed;

        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.LocaleChanged += HandleLocaleChanged;
    }

    private void OnDisable()
    {
        // 取消订阅，防止内存泄漏
        TaskEventBus.OnTaskStarted -= HandleTaskStarted;
        TaskEventBus.OnTaskAdvanced -= HandleTaskAdvanced;
        TaskEventBus.OnTaskCompleted -= HandleTaskCompleted;
        TaskEventBus.OnTaskFailed -= HandleTaskFailed;

        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.LocaleChanged -= HandleLocaleChanged;
    }

    private void Update()
    {
        // Tab 键切换面板显示/隐藏
        if (Input.GetKeyDown(KeyCode.Tab))
            TogglePanel();
    }

    // ─────────────────────────────────────────────
    // 事件处理
    // ─────────────────────────────────────────────

    private void HandleTaskStarted(TaskRuntimeData data)
    {
        UpdateTexts(data);
        SlideIn(); // 任务启动时滑入
    }

    private void HandleTaskAdvanced(TaskRuntimeData data)
    {
        UpdateTexts(data); // 仅更新文本，不触发动画
    }

    private void HandleTaskCompleted(TaskRuntimeData data)
    {
        UpdateTexts(data);
        SlideOut(); // 任务完成后滑出
    }

    private void HandleTaskFailed(TaskRuntimeData data)
    {
        UpdateTexts(data);
        SlideOut(); // 任务失败后滑出
    }

    // ─────────────────────────────────────────────
    // UI 更新
    // ─────────────────────────────────────────────

    /// <summary>根据任务运行时数据更新所有文本内容。</summary>
    private void UpdateTexts(TaskRuntimeData data)
    {
        _currentData = data;

        string title = string.Empty;
        string content = string.Empty;

        if (LocalizationManager.Instance != null)
        {
            string raw = LocalizationManager.Instance.GetLocalizedString(
                LocalizationManager.TableName.TaskTable,
                data.Definition.taskLocalizationKey);

            if (!string.IsNullOrEmpty(raw))
                LocalizationManager.Instance.GetLocalizedTaskInfo(raw, out title, out content);
        }

        if (nameText     != null) nameText.text     = title;
        if (descText     != null) descText.text     = content;
        if (progressText != null) progressText.text = $"{data.CurrentProgress} / {data.Definition.totalProgress}";
    }

    /// <summary>语言切换时刷新当前显示的任务文本。</summary>
    private void HandleLocaleChanged(LocalizationManager.LocaleID _)
    {
        if (_currentData != null)
            UpdateTexts(_currentData);
    }

    // ─────────────────────────────────────────────
    // 动画控制
    // ─────────────────────────────────────────────

    /// <summary>将面板从屏幕左侧滑入。</summary>
    private void SlideIn()
    {
        if (_isVisible) return; // 已显示则跳过
        StartSlide(targetX: 960f);
        _isVisible = true;
    }

    /// <summary>将面板滑出到屏幕左侧。</summary>
    private void SlideOut()
    {
        if (!_isVisible) return; // 已隐藏则跳过
        StartSlide(targetX: -taskPanel.rect.width);
        _isVisible = false;
    }

    /// <summary>Tab 键切换：若可见则滑出，若隐藏则滑入。</summary>
    private void TogglePanel()
    {
        if (_isVisible)
            SlideOut();
        else
            SlideIn();
    }

    /// <summary>启动滑动协程，若已有协程则先停止（打断并重新开始）。</summary>
    private void StartSlide(float targetX)
    {
        if (_slideCoroutine != null)
            StopCoroutine(_slideCoroutine); // 打断当前动画

        _slideCoroutine = StartCoroutine(SlideCoroutine(targetX));
    }

    /// <summary>
    /// 平滑插值滑动协程。
    /// 使用 Lerp 在 slideDuration 秒内将面板移动到目标 X 位置。
    /// </summary>
    private IEnumerator SlideCoroutine(float targetX)
    {
        Vector2 startPos = taskPanel.anchoredPosition;
        Vector2 endPos = new Vector2(targetX, startPos.y);
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t); // 缓入缓出曲线

            taskPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            yield return null; // 等待下一帧
        }

        taskPanel.anchoredPosition = endPos; // 确保精确到达目标位置
        _slideCoroutine = null;
    }
}
