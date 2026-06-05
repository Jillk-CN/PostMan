## 提案：VHSPanel 关闭后返回设置面板

### 做什么

在 `VHSPanel` 关闭时自动重新显示 `SettingsPanel`，让用户从 VHS 滤镜三级面板关闭后
能继续操作设置二级面板，而不是陷入"设置面板和主菜单都不可见"的死角。

### 为什么

当前导航链：主菜单 → 设置面板 → VHSPanel。
`SettingsPanel.OnOpenVHS()` 只调用 `VHSPanel.Instance.Show()`，没有先隐藏自身；
而 `VHSPanel.Hide()` 只恢复输入状态、关闭自身，没有通知调用方恢复显示。
关闭 VHSPanel 后，两个面板都处于活跃或隐藏的不一致状态（取决于 SettingsPanel
是否在打开 VHS 前隐藏了自身），用户无法返回设置继续操作。

### 非目标

- 不重构整体 UI 导航框架
- 不添加面板栈或通用的 Back 机制
- 不改变游戏场景中 VHSPanel 的现有行为
- 不修改 TitleUIManager 的导航逻辑

### 方案

给 `VHSPanel` 增加一个 `public event Action OnHide`，在 `Hide()` 末尾触发。
`SettingsPanel` 在 `OnEnable`/`OnDisable` 中订阅/取消该事件，
收到回调时重新激活自身（`SetActive(true)`）。

这种方式让 VHSPanel 完全解耦于调用方，游戏场景内没有订阅者时事件安全空发，
不影响现有功能。
