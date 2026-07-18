## Proposal: Gamma 值接入 URP Volume Post-Processing

### What

将 `SettingsPanel` 中 Gamma 滑条的实现从"仅保存 PlayerPrefs"补全为"实时驱动 URP Volume 的 `ColorAdjustments.postExposure` 参数"，让画面亮度调节真正生效。

### Why

`SettingsPanel.OnGammaChanged()` 目前只写入 `PlayerPrefs`，带有一条 `// TODO` 注释，没有任何渲染效果。`SampleSceneProfile` 中也不存在 `ColorAdjustments` 组件，导致 Gamma 滑条完全失效。同时，`SyncUIFromPrefs()` 在面板打开时会从 PlayerPrefs 还原滑条值，但对应的 Volume 参数却没有同步恢复，首次打开游戏时渲染结果与 UI 显示不一致。

### Non-goals

- 不使用 `LiftGammaGain`（曲线级别的色彩分级，超出当前需求）
- 不为 Gamma 新建独立 `GammaController` 脚本（复杂度不足以单独拆文件）
- 不修改 VHSFilterController 的 VHSSettings 数据模型（Gamma 是独立的画面设置，不属于 VHS 效果）

### Approach

1. 在 `SampleSceneProfile` 中添加 `ColorAdjustments` Volume 组件（Editor 手动操作）。
2. 新建轻量辅助类 `GammaVolumeController`，单例持有目标 Volume 引用，对外暴露 `Apply(float gamma)` 方法，将 0.5–2.0 的线性 Slider 值映射为 `postExposure`（EV 值）后写入 `ColorAdjustments`。
3. `SettingsPanel.OnGammaChanged()` 在保存 PlayerPrefs 之后调用 `GammaVolumeController.Instance?.Apply(value)`。
4. `SettingsPanel.SyncUIFromPrefs()` 在还原滑条值之后同样调用一次 `Apply`，保证启动时渲染与 UI 一致。
