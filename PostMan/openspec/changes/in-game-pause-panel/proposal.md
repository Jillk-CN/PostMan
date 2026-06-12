## Proposal: 游戏内暂停面板

### What
实现游戏内暂停面板，包含 ESC 呼出/关闭、背景虚化、返回游戏/设置/返回主菜单三个按钮。

### Why
游戏已有 PauseInputSource（ESC 键）但没有任何消费方。玩家进入游戏后无法暂停，体验缺失。

### Non-goals
- 存档功能
- 暂停时的过场动画

### Approach
单一 PausePanel MonoSingleton，遵循现有 TextPanel/ViewImagePanel 的 Show/Hide 模式。
背景虚化通过 URP Volume + Depth of Field 实现，避免引入额外依赖。
SettingsPanel 注入返回回调，兼容标题场景和游戏内场景双重调用方。
