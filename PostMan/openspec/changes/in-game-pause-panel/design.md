## Design: 游戏内暂停面板

### 组件架构

```
PausePanel (MonoSingleton<PausePanel>)
  ├── SerializeField
  │   ├── SettingsPanel settingsPanel     // 游戏内设置面板引用（共用 SettingsPanel 组件）
  │   ├── List<string> titleScenesToLoad  // 返回主菜单时加载的场景
  │   ├── List<string> titleScenesToUnload// 返回主菜单时卸载的场景
  │   ├── Volume pauseVolume             // 专用 URP Volume（含 Depth of Field）
  │   ├── Button btnResumeGame           // 返回游戏按钮
  │   ├── Button btnSettings             // 设置按钮
  │   └── Button btnReturnToTitle        // 返回主菜单按钮
  └── 依赖
      ├── GameInputManager.Instance
      ├── PauseInputSource（通过 GameInputManager 获取）
      └── GameSceneManager.Instance
```

### Show() 逻辑
1. `gameObject.SetActive(true)`
2. `Time.timeScale = 0f`
3. `pauseVolume.enabled = true`
4. `GameInputManager.Instance.DisablePlayerAllInput()`
5. `GameInputManager.Instance.ShowCursor()`
6. `GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(false)`

### Hide() 逻辑
1. `gameObject.SetActive(false)`
2. `Time.timeScale = 1f`
3. `pauseVolume.enabled = false`
4. `GameInputManager.Instance.EnablePlayerAllInput()`
5. `GameInputManager.Instance.HideCursor()`

### Update() 逻辑
- `PauseInputSource.GetPause() && !gameObject.activeSelf` → `Show()`
- `PauseInputSource.GetPause() && gameObject.activeSelf` → `Hide()`

### SettingsPanel 适配
`SettingsPanel.onBack` 委托：
- PausePanel.OnOpenSettings() 注入：隐藏设置面板 → 显示暂停面板
- 标题场景不注入 → 默认调用 TitleUIManager.ReturnToMainMenu()

### 背景虚化
- 游戏场景新建 `PauseVolume` GameObject
- 挂载 URP Global Volume 组件
- Profile 添加 Depth of Field（Bokeh 模式）
- 默认 disabled，Show() 时启用，Hide() 时禁用
