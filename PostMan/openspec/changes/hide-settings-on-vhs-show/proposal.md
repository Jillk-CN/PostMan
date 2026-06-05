## 提案：VHSPanel 显示时隐藏 SettingsPanel

### 做什么

在 `SettingsPanel` 打开 `VHSPanel` 时，先隐藏自身，防止两个面板同时可见。

### 为什么

上一个变更（vhs-panel-back-to-settings）已实现 VHSPanel 关闭后返回 SettingsPanel。
但 `OnOpenVHS()` 只调用 `VHSPanel.Instance.Show()`，没有先隐藏 SettingsPanel，
导致打开 VHSPanel 时两个面板同时可见，出现 UI 叠层问题。

### 非目标

- 不重构整体 UI 导航框架
- 不添加面板栈或通用导航机制
- 不修改 VHSPanel 本身

### 方案

调整 `SettingsPanel` 的事件订阅时机（从 `OnEnable`/`OnDisable` 改为 `Awake`/`OnDestroy`），
并在 `OnOpenVHS()` 中先 `SetActive(false)` 隐藏自身，再调用 `VHSPanel.Instance.Show()`。

订阅时机迁移的原因：`SetActive(false)` 会触发 `OnDisable`，若此时取消订阅，
VHSPanel 关闭后将无法收到 `OnHide` 事件、无法重新显示 SettingsPanel。
将订阅移至 `Awake`/`OnDestroy` 可确保整个生命周期内事件订阅持续有效。
