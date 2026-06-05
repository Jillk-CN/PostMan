## 设计：按钮悬停字体切换

### 文件

```
Assets/Scripts/UI/
└── ButtonHoverFontSwap.cs   (~50 行)
```

### 依赖（复用，不修改）

| 文件 | 用途 |
|---|---|
| `Assets/Scripts/Common/UGUIEventListener.cs` | 提供 `PointerEnter` / `PointerExit` 事件 |

### 代码结构

```csharp
namespace PostMan.UI
{
    /// <summary>鼠标悬停时切换按钮 TMP_Text 字体，离开时恢复。</summary>
    [RequireComponent(typeof(UGUIEventListener))]
    public class ButtonHoverFontSwap : MonoBehaviour
    {
        [Header("字体配置")]
        [SerializeField] private TMP_Text      _label;
        [SerializeField] private TMP_FontAsset _defaultFont;
        [SerializeField] private TMP_FontAsset _hoverFont;

        private UGUIEventListener _listener;

        private void Awake()     => _listener = GetComponent<UGUIEventListener>();
        private void OnEnable()  { _listener.PointerEnter += OnEnter; _listener.PointerExit += OnExit; }
        private void OnDisable() { _listener.PointerEnter -= OnEnter; _listener.PointerExit -= OnExit; }

        private void OnEnter(UGUIEventListener s, PointerEventData e) => _label.font = _hoverFont;
        private void OnExit (UGUIEventListener s, PointerEventData e) => _label.font = _defaultFont;
    }
}
```

### 设计要点

- **解耦**：组件自包含，不依赖 `TitleUIManager` / `LanguagePanel` / `SettingsPanel`
- **`[RequireComponent]`**：保证 `UGUIEventListener` 自动存在，无需手动添加
- **`OnEnable`/`OnDisable`**：面板切换时订阅正确重建，无悬空委托
- **无内部状态**：仅序列化字段，字体切换即时发生，无需状态追踪
- **`_label` 显式引用**：Inspector 可见，避免 `GetComponentInChildren` 的脆弱性

### Inspector 连线

| 字段 | 赋值 |
|---|---|
| `_label` | 按钮的 `Text (TMP)` 子物体 |
| `_defaultFont` | 默认字体资产 |
| `_hoverFont` | 悬停强调字体资产（加粗/斜体等） |
