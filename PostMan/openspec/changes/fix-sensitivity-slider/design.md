## Design: 修复设置面板灵敏度 Slider 不生效问题

### 根本原因

```
SettingsPanel.OnSensitivityChanged()
    └─ PlayerPrefs.SetFloat("MouseSensitivity", value)  ✓ 写入正确
       PlayerPrefs.Save()                                ✓ 落盘正确
       // ← 缺少：将值推送给运行中的 PlayerSight 实例
```

`PlayerSight.Start()` 只在场景加载时执行一次，之后 `sensitivity` 字段不再更新，所以游戏内调整滑块无效。

### 修复方案

**仅修改一个文件**：`Assets/Scripts/UI/Title/SettingsPanel.cs`

在 `OnSensitivityChanged(float value)` 方法中，PlayerPrefs 写入之后追加一次对场景内 `PlayerSight` 的实时推送：

```csharp
private void OnSensitivityChanged(float value)
{
    PlayerPrefs.SetFloat(KeySensitivity, value);
    PlayerPrefs.Save();
    // 若游戏场景中存在 PlayerSight，立即推送新值（游戏内暂停菜单调整时生效）
    // 标题场景无玩家，FindObjectOfType 返回 null，此处安全跳过
    var sight = Object.FindObjectOfType<PostMan.Player.PlayerSight>();
    if (sight != null) sight.sensitivity = value;
}
```

需要在文件顶部补充 `using PostMan.Player;`（若尚未引入）。

### 为何不用其他方案

| 方案 | 原因放弃 |
|------|---------|
| `PlayerSight.Update()` 每帧读取 PlayerPrefs | 有持续性能开销，且 PlayerPrefs 不是为高频读取设计的 |
| 静态事件 / EventBus | 引入新架构，改动范围超出 bug 修复范畴 |
| 给 `PlayerSight` 增加 `ApplySensitivity()` 方法 | 多余的抽象，直接赋公开字段更简洁 |

### 关键文件

| 文件 | 改动类型 |
|------|---------|
| `Assets/Scripts/UI/Title/SettingsPanel.cs` | 在 `OnSensitivityChanged` 追加 2 行，可能需要新增 using |

### 不影响的逻辑

- 标题场景行为：`FindObjectOfType<PlayerSight>()` 返回 `null`，完全跳过，PlayerPrefs 流程与现在相同
- `PlayerSight.Start()` 逻辑不变，游戏场景首次加载仍从 PlayerPrefs 读取
- `SyncUIFromPrefs()` 逻辑不变，面板显示值依然正确回显
