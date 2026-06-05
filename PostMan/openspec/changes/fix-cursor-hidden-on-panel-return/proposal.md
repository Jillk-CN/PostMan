## 提案：修复 VHSPanel 关闭后鼠标被隐藏导致 UI 无法交互

### 做什么

修复从 SettingsPanel 打开并关闭 VHSPanel 后，鼠标光标被隐藏（锁定）导致
无法与 SettingsPanel 及其他 UI 元素交互的问题。
同时确保所有 UI 面板处于显示状态时，鼠标保持可见。

### 为什么

`VHSPanel.Hide()` 无条件调用 `GameInputManager.HideCursor()`（锁定鼠标），
这在游戏场景下是正确的（关闭 VHSPanel 后应恢复玩家操控）。
但在标题场景中，从 SettingsPanel 打开 VHSPanel 后再关闭，
`HideCursor()` 覆盖了 `TitleUIManager.Start()` 设置的"鼠标常显"状态，
导致返回 SettingsPanel 后鼠标不可见，用户无法点击任何 UI 控件。

### 非目标

- 不重构 GameInputManager 的整体光标管理机制
- 不影响游戏场景中 VHSPanel 的现有行为（游戏场景里关闭 VHS 应仍然隐藏鼠标）
- 不改变 TitleUIManager 的导航逻辑

### 方案

在 `VHSPanel.Hide()` 的末尾，用 `OnHide` 事件通知调用方自行决定是否恢复光标，
而不是由 VHSPanel 无条件 `HideCursor()`。

具体实现：
- `SettingsPanel.OnVHSPanelClosed()` 在 `SetActive(true)` 之后主动调用
  `GameInputManager.Instance.ShowCursor()`，恢复标题场景所需的鼠标显示状态。
- `VHSPanel.Hide()` 保持 `HideCursor()` 调用（游戏场景行为不变）；
  `SettingsPanel` 在收到回调后再次覆写为 `ShowCursor()`，顺序上无冲突。

这样最小化改动，无需引入新接口或"调用来源"标志。
