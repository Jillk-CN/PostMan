## 任务清单：修复 VHSPanel 关闭后鼠标隐藏问题

### Task 1 — 修改 SettingsPanel.OnVHSPanelClosed()

**文件：** `Assets/Scripts/UI/Title/SettingsPanel.cs`

**修改内容：**

在 `OnVHSPanelClosed()` 方法中，`SetActive(true)` 之后追加
`GameInputManager.Instance.ShowCursor()` 调用。

**修改前：**
```csharp
private void OnVHSPanelClosed()
{
    this.gameObject.SetActive(true);
}
```

**修改后：**
```csharp
/// <summary>VHSPanel 关闭后重新激活本面板并恢复鼠标显示。</summary>
private void OnVHSPanelClosed()
{
    this.gameObject.SetActive(true);
    GameInputManager.Instance.ShowCursor();   // 恢复标题场景的鼠标显示状态
}
```

**验收：**
- Unity 编译无报错
- 标题场景：主菜单 → 设置 → 打开 VHS → 关闭 VHS → SettingsPanel 显示，鼠标可见可点击
- 多次循环打开/关闭 VHS，鼠标状态始终正确
- 游戏场景：VHSPanel 通过 O 键开关，关闭后鼠标仍然锁定（行为不变）
