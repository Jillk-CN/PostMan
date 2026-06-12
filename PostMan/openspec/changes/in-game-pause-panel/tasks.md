## Tasks: 游戏内暂停面板

### 脚本
- [x] 修改 `Assets/Scripts/UI/Title/SettingsPanel.cs`：添加 `public Action onBack` 字段，`OnBack()` 改为委托优先
- [x] 创建 `Assets/Scripts/UI/PausePanel.cs`：完整实现（含 Show/Hide/Update/按钮回调）

### 预制体 & 场景配置（Inspector 手动操作）
- [ ] 在游戏场景中创建 `PauseVolume` GameObject，挂载 Global Volume + DOF Profile（Bokeh）
- [ ] 创建 `PausePanel` Canvas/预制体，搭建 3 个按钮 UI：返回游戏、设置、返回主菜单
- [ ] 将 `PausePanel.cs` 挂载到预制体，在 Inspector 中赋值所有 SerializeField：
  - settingsPanel → 场景中的 SettingsPanel 实例
  - titleScenesToLoad → 主菜单场景地址列表
  - titleScenesToUnload → 当前游戏场景地址列表
  - pauseVolume → 上面创建的 PauseVolume
  - 三个按钮引用

### Verification
1. Play Mode 进入游戏场景
2. 按 ESC → 面板出现、背景虚化、鼠标可见、角色不动（timeScale=0）
3. 再按 ESC 或点"返回游戏" → 面板消失、虚化取消、鼠标隐藏、游戏恢复
4. 按 ESC，点"设置" → 设置面板出现，暂停面板隐藏
5. 设置面板点"返回" → 回到暂停面板（非主菜单）
6. 按 ESC，点"返回主菜单" → 场景切换到标题，鼠标可见，timeScale=1
7. 标题场景的设置面板"返回"功能正常（兼容性回归）
8. Console 无错误
