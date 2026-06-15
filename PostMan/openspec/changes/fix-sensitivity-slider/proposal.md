## Proposal: 修复设置面板灵敏度 Slider 不生效问题

### What

修复 `SettingsPanel` 中灵敏度 Slider 滑动后 `PlayerSight.sensitivity` 不实时更新的问题，使调整在标题场景和游戏内暂停场景下均能立即生效。

### Why

当前 `PlayerSight.sensitivity` 只在 `Start()` 中从 PlayerPrefs 读取一次。`SettingsPanel.OnSensitivityChanged()` 虽然正确写入了 PlayerPrefs，但在以下两种情况下无法生效：

1. **游戏内暂停菜单调整**：`PlayerSight` 已在游戏场景运行，`Start()` 不会再次执行，改动无法应用到正在运行的实例。
2. **标题场景→游戏场景**：理论上应该生效（游戏场景 `Start()` 会读取 PlayerPrefs），但如果存在场景切换时机问题或 PlayerPrefs 未及时落盘，也可能出现不生效。

### Non-goals

- 不修改灵敏度的取值范围或 UI 布局
- 不调整 PlayerPrefs 键名或序列化结构
- 不涉及抖动（EnableShake）相关逻辑

### Approach

在 `SettingsPanel.OnSensitivityChanged()` 中，除写入 PlayerPrefs 外，额外通过 `FindObjectOfType<PlayerSight>()` 尝试获取场景中的 `PlayerSight` 实例，如果找到则直接赋值 `sensitivity` 字段，实现游戏内实时生效。标题场景中不存在 `PlayerSight`，`FindObjectOfType` 返回 `null`，此时仅保留 PlayerPrefs 写入，行为与现在完全一致。
