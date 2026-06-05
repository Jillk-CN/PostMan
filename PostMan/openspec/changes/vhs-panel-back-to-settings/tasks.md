## 任务：VHSPanel 关闭后返回设置面板

### 实现任务

- [ ] 修改 `Assets/Scripts/UI/VHSPanel.cs`
  - 文件顶部添加 `using System;`
  - 在 `_filterController` 字段声明之后新增事件：
    `public event Action OnHide;`
  - `Hide()` 方法末尾（`HideCursor()` 之后）追加：`OnHide?.Invoke();`

- [ ] 修改 `Assets/Scripts/UI/Title/SettingsPanel.cs`
  - `OnEnable()` 末尾追加：
    ```csharp
    if (VHSPanel.Instance != null)
        VHSPanel.Instance.OnHide += OnVHSPanelClosed;
    ```
  - 新增 `OnDisable()` 方法（类中当前不存在）：
    ```csharp
    private void OnDisable()
    {
        if (VHSPanel.Instance != null)
            VHSPanel.Instance.OnHide -= OnVHSPanelClosed;
    }
    ```
  - 新增私有回调方法（放在"控件响应：VHS / 导航"区域）：
    ```csharp
    /// <summary>VHSPanel 关闭后重新激活本面板。</summary>
    private void OnVHSPanelClosed()
    {
        this.gameObject.SetActive(true);
    }
    ```

### 验证步骤

- [ ] Unity 编译无报错、无警告
- [ ] 标题场景 Play 模式：
  - 主菜单 → 设置 → 打开 VHS → 关闭 VHS → SettingsPanel 重新可见
  - 在 SettingsPanel 中操作其他控件（灵敏度、帧率等）仍正常
  - 再次打开 VHS → 关闭 → 仍返回 SettingsPanel（多次循环）
- [ ] 游戏场景（如有）：触发 VHSPanel.Show/Hide 不报错，不因 OnHide 订阅为空而崩溃
