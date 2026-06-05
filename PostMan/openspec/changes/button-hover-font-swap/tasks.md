## 任务：按钮悬停字体切换

### 实现任务

- [ ] 创建 `Assets/Scripts/UI/ButtonHoverFontSwap.cs`
  - 命名空间 `PostMan.UI`，继承 `MonoBehaviour`
  - 添加 `[RequireComponent(typeof(UGUIEventListener))]`
  - 三个 `[SerializeField]` 字段：`TMP_Text _label`、`TMP_FontAsset _defaultFont`、`TMP_FontAsset _hoverFont`
  - `Awake()`：缓存 `_listener = GetComponent<UGUIEventListener>()`
  - `OnEnable()`：订阅 `_listener.PointerEnter += OnPointerEnter` 和 `_listener.PointerExit += OnPointerExit`
  - `OnDisable()`：取消订阅两个事件
  - `OnPointerEnter`：执行 `_label.font = _hoverFont`
  - `OnPointerExit`：执行 `_label.font = _defaultFont`
  - 所有公开类和方法添加中文 XML `<summary>` 文档注释
  - 三个序列化字段加 `[Tooltip]`，用 `[Header("字体配置")]` 分组

### Unity Editor 配置步骤

- [ ] 对标题场景中每个需要悬停效果的按钮 GameObject：
  - 在 Hierarchy 中选中按钮 GameObject
  - 添加组件 `ButtonHoverFontSwap`（若缺少 `UGUIEventListener` 则 Unity 自动补充）
  - 将 `_label` 连线 → 拖入该按钮的 `Text (TMP)` 子物体
  - 将 `_defaultFont` 连线 → 按钮平时使用的字体资产
  - 将 `_hoverFont` 连线 → 强调字体资产（如加粗或斜体变体）
- [ ] 进入 Play 模式验证：悬停按钮时字体切换，移开后恢复
- [ ] 验证面板重新打开后 `LanguagePanel` 和 `SettingsPanel` 上的按钮仍正常响应
  （验证 `OnEnable`/`OnDisable` 生命周期的正确性）
