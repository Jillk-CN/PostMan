## 设计：VHSPanel 关闭后返回设置面板

### 涉及文件

| 文件 | 操作 |
|---|---|
| `Assets/Scripts/UI/VHSPanel.cs` | 添加 `OnHide` 事件，在 `Hide()` 末尾触发 |
| `Assets/Scripts/UI/Title/SettingsPanel.cs` | `OnEnable`/`OnDisable` 订阅/取消 `OnHide`，回调中重新激活自身 |

### VHSPanel.cs 修改

新增 `using System;` 引用。
在类体内新增事件声明：

```csharp
/// <summary>VHSPanel 关闭时触发，供调用方（如 SettingsPanel）重新显示自身。</summary>
public event Action OnHide;
```

`Hide()` 末尾追加触发：

```csharp
public void Hide()
{
    this.gameObject.SetActive(false);
    GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(true);
    GameInputManager.Instance.EnablePlayerAllInput();
    GameInputManager.Instance.HideCursor();
    OnHide?.Invoke();   // 通知订阅方
}
```

### SettingsPanel.cs 修改

`OnEnable()` 末尾追加订阅（在已有 `SyncUIFromPrefs()` 之后）：

```csharp
if (VHSPanel.Instance != null)
    VHSPanel.Instance.OnHide += OnVHSPanelClosed;
```

`OnDisable()` 追加（当前该方法不存在，需新增）：

```csharp
private void OnDisable()
{
    if (VHSPanel.Instance != null)
        VHSPanel.Instance.OnHide -= OnVHSPanelClosed;
}
```

新增回调方法：

```csharp
/// <summary>VHSPanel 关闭后重新激活本面板。</summary>
private void OnVHSPanelClosed()
{
    this.gameObject.SetActive(true);
}
```

### 设计要点

- **解耦**：`VHSPanel` 不引用 `SettingsPanel` 或 `TitleUIManager`，事件无订阅者时安全空发
- **`OnEnable`/`OnDisable` 订阅**：面板激活/停用时自动建立/断开，无悬空委托
- **最小改动**：不触及 `TitleUIManager`，不引入新类，改动集中在两个文件共 ~6 行
- **游戏场景兼容**：`VHSPanel` 在游戏场景依然可用，`OnHide` 无订阅者不影响功能
