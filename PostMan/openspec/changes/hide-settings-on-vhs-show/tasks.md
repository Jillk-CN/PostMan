## 任务：VHSPanel 显示时隐藏 SettingsPanel

### 实现任务

- [ ] 修改 `Assets/Scripts/UI/Title/SettingsPanel.cs`
  - `Awake()` 末尾追加：
    ```csharp
    if (VHSPanel.Instance != null)
        VHSPanel.Instance.OnHide += OnVHSPanelClosed;
    ```
  - `OnDestroy()` 末尾追加：
    ```csharp
    if (VHSPanel.Instance != null)
        VHSPanel.Instance.OnHide -= OnVHSPanelClosed;
    ```
  - `OnEnable()` 中移除订阅行，只保留 `SyncUIFromPrefs()`
  - 删除 `OnDisable()` 方法（或保留空方法体）
  - `OnOpenVHS()` 改为先 `SetActive(false)` 再 `VHSPanel.Instance.Show()`

### 验证步骤

- [ ] Unity 编译无报错、无警告
- [ ] 标题场景 Play 模式：
  - 主菜单 → 设置 → 点击打开 VHS → SettingsPanel 隐藏，只显示 VHSPanel
  - 关闭 VHS → SettingsPanel 重新出现，VHSPanel 隐藏
  - 多次循环（打开/关闭 VHS ×3）仍正常
  - `SyncUIFromPrefs()` 每次重新显示 SettingsPanel 时正确同步控件值
- [ ] 游戏场景（如有）：触发 VHSPanel.Show/Hide 不报错
