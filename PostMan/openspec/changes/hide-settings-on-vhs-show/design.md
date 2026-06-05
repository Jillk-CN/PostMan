## 设计：VHSPanel 显示时隐藏 SettingsPanel

### 涉及文件

| 文件 | 操作 |
|---|---|
| `Assets/Scripts/UI/Title/SettingsPanel.cs` | 迁移 `OnHide` 订阅至 `Awake`/`OnDestroy`；`OnOpenVHS` 先隐藏自身再显示 VHSPanel |

### SettingsPanel.cs 修改

#### 1. `Awake()` — 追加订阅

```csharp
private void Awake()
{
    BindButtons();
    BindControls();
    if (VHSPanel.Instance != null)
        VHSPanel.Instance.OnHide += OnVHSPanelClosed;
}
```

#### 2. `OnDestroy()` — 追加取消订阅

```csharp
private void OnDestroy()
{
    UnbindButtons();
    UnbindControls();
    if (VHSPanel.Instance != null)
        VHSPanel.Instance.OnHide -= OnVHSPanelClosed;
}
```

#### 3. `OnEnable()` — 移除订阅行，只保留 `SyncUIFromPrefs()`

```csharp
private void OnEnable()
{
    SyncUIFromPrefs();
}
```

#### 4. `OnDisable()` — 移除取消订阅行（整个方法可删除，内容为空）

#### 5. `OnOpenVHS()` — 先隐藏自身，再显示 VHSPanel

```csharp
private void OnOpenVHS()
{
    if (VHSPanel.Instance == null)
    {
        Debug.LogWarning("[SettingsPanel] VHSPanel 实例不存在，请在场景中放置 VHSPanel 预制体。");
        return;
    }
    this.gameObject.SetActive(false);
    VHSPanel.Instance.Show();
}
```

### 设计要点

- **订阅生命周期**：迁移至 `Awake`/`OnDestroy`，面板激活/停用不影响事件订阅，避免 `SetActive(false)` 时丢失订阅
- **`OnEnable` 仍有效**：`OnVHSPanelClosed` 触发 `SetActive(true)` 后，`OnEnable` 自动执行 `SyncUIFromPrefs()`，UI 数据仍保持同步
- **最小改动**：仅修改 `SettingsPanel.cs`，不触及 `VHSPanel` 或 `TitleUIManager`
- **游戏场景兼容**：`VHSPanel` 在游戏场景中依然正常工作，无副作用
