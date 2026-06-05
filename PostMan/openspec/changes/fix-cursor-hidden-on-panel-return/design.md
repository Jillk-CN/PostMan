## 设计：修复 VHSPanel 关闭后鼠标隐藏问题

### 根本原因

```
SettingsPanel.OnOpenVHS()
  → gameObject.SetActive(false)      // 隐藏 SettingsPanel
  → VHSPanel.Instance.Show()         // ShowCursor() ✓ 鼠标可见
     ↓ 用户关闭 VHSPanel
VHSPanel.Hide()
  → gameObject.SetActive(false)
  → HideCursor()                     // ← 无条件锁定鼠标 ✗
  → OnHide?.Invoke()
     ↓
SettingsPanel.OnVHSPanelClosed()
  → gameObject.SetActive(true)       // SettingsPanel 重新显示
  // 鼠标仍然锁定，无法交互          ← 问题根源
```

### 修复方案

在 `SettingsPanel.OnVHSPanelClosed()` 中，`SetActive(true)` 之后立即调用
`GameInputManager.Instance.ShowCursor()`，覆写 `Hide()` 的 `HideCursor()`。

```csharp
// Assets/Scripts/UI/Title/SettingsPanel.cs

/// <summary>VHSPanel 关闭后重新激活本面板并恢复鼠标显示。</summary>
private void OnVHSPanelClosed()
{
    this.gameObject.SetActive(true);
    GameInputManager.Instance.ShowCursor();   // 恢复标题场景的鼠标显示状态
}
```

### 为什么不改 VHSPanel.Hide()

`VHSPanel.Hide()` 的 `HideCursor()` 在游戏场景中是正确行为——关闭 VHSPanel
后玩家应立即恢复操控（鼠标锁定）。如果从 VHSPanel 侧判断"调用来源是标题场景还是游戏场景"
则需引入额外状态，增加耦合。

让 `SettingsPanel`（标题场景专属组件）在回调里自行恢复光标，是最小改动、最低耦合的做法。

### 调用链修复后

```
VHSPanel.Hide()
  → HideCursor()                     // 游戏场景行为正常
  → OnHide?.Invoke()
     ↓
SettingsPanel.OnVHSPanelClosed()
  → SetActive(true)                  // SettingsPanel 重新显示
  → ShowCursor()                     // ✓ 标题场景鼠标恢复可见
```

### 影响范围

| 文件 | 修改内容 |
|---|---|
| `Assets/Scripts/UI/Title/SettingsPanel.cs` | `OnVHSPanelClosed()` 追加 `ShowCursor()` 调用 |

其他文件（VHSPanel、TitleUIManager、GameInputManager）**不需要修改**。

### 扩展性说明

若未来其他面板也需要从 VHSPanel 返回后显示鼠标，同样在各自的 `OnHide` 回调中
调用 `ShowCursor()` 即可，模式一致，无需修改 VHSPanel 本身。
