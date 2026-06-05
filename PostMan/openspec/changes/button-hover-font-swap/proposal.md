## 提案：按钮悬停字体切换

### 做什么

新增一个可复用的 `ButtonHoverFontSwap` 组件，在鼠标进入按钮时将按钮的 `TMP_Text`
子物体的 `TMP_FontAsset` 切换为强调字体，鼠标离开时恢复默认字体，实现即时视觉强调效果。

### 为什么

标题场景主菜单的各按钮目前没有任何悬停反馈。在悬停时切换为独特字体是一种低成本、
视觉感强的方式来提示当前聚焦按钮，比单纯改变颜色更贴合游戏的复古/打字机美学。

### 非目标

- 不做动画过渡（字体切换为即时发生）
- 不支持键盘/手柄导航高亮（仅鼠标指针）
- 不修改现有按钮点击逻辑
- 不新增字体资产（字体由设计师在 Inspector 中指定）
- 不修改 `TitleUIManager`、`LanguagePanel` 或 `SettingsPanel`

### 方案

创建单一自包含的 `ButtonHoverFontSwap : MonoBehaviour` 组件，挂载在每个需要悬停效果
的按钮 GameObject 上。复用已有的 `UGUIEventListener` 组件（通过 `[RequireComponent]`
自动保证存在），在 `OnEnable` / `OnDisable` 中订阅/取消 `PointerEnter` / `PointerExit`
事件，遵循项目中 `ImageViewer.cs` 已建立的模式。
