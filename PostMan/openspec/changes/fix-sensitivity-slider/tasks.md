## Tasks: 修复设置面板灵敏度 Slider 不生效问题

### 实施步骤

- [ ] **Task 1**：修改 `Assets/Scripts/UI/Title/SettingsPanel.cs`
  - 在文件顶部确认是否已有 `using PostMan.Player;`，没有则新增
  - 在 `OnSensitivityChanged(float value)` 方法的 `PlayerPrefs.Save()` 之后追加：
    ```csharp
    var sight = Object.FindObjectOfType<PostMan.Player.PlayerSight>();
    if (sight != null) sight.sensitivity = value;
    ```

### 验证方式

1. **游戏内实时生效**：进入游戏场景，打开暂停/设置面板，拖动灵敏度 Slider，关闭面板后立即转动鼠标，确认灵敏度已按新值响应。
2. **标题场景无报错**：在标题场景打开设置面板拖动 Slider，确认控制台无 NullReferenceException。
3. **跨场景持久化**：在标题场景调整灵敏度，进入游戏场景后确认 `PlayerSight.sensitivity` 值与设置一致（`Start()` 读取路径正常工作）。
4. **重启持久化**：退出游戏重进，确认灵敏度仍为上次设置的值。
