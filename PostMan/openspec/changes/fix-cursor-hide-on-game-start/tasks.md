# Tasks: 修复进入游戏场景后鼠标未隐藏的问题

## Implementation Tasks

- [ ] 在 `Assets/Scripts/UI/Title/TitleUIManager.cs` 的 `OnStartGame()` 方法中，在调用 `GameSceneManager.Instance.SwitchScenes(...)` 之前添加一行：
  ```csharp
  GameInputManager.Instance.HideCursor();
  ```

## Verification

- [ ] 在 Unity Editor 中进入 Play Mode，停在标题场景
- [ ] 确认标题场景内鼠标可见（现有行为正常）
- [ ] 点击 Start 按钮，等待游戏场景加载完成
- [ ] 确认进入游戏场景后鼠标光标被隐藏且锁定（不再可见）
- [ ] 确认 Console 无相关错误或警告
