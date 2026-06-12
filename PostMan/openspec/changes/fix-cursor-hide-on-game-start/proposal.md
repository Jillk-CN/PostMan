## Proposal: 修复进入游戏场景后鼠标未隐藏的问题

### What

点击标题页面的 Start 按钮后，切换至游戏场景时鼠标光标未被隐藏，仍然可见且未锁定。

### Why

`TitleUIManager.Start()` 调用了 `GameInputManager.Instance.ShowCursor()` 以确保标题场景内鼠标可见。但 `OnStartGame()` 在触发场景切换时没有调用 `HideCursor()`，导致光标在游戏场景加载后仍保持显示状态。

`GameInputManager.Init()` 中确实调用了 `HideCursor()`，但该方法只在单例首次创建时执行一次，此后不会重复触发。`TitleUIManager` 标注了 `DontDestroyOnLoad`，因此切换场景时该对象不会销毁，`ShowCursor()` 的状态也被保留到了游戏场景中。

### Approach

在 `TitleUIManager.OnStartGame()` 中，于调用 `SwitchScenes` 之前调用 `GameInputManager.Instance.HideCursor()`，确保进入游戏场景时光标立即被隐藏并锁定。

### Non-goals

- 不修改 `GameSceneManager`，其职责与光标管理无关
- 不修改 `GameInputManager` 的初始化逻辑
- 不为其他场景切换路径（`ColliderSceneChange`、`InteractableSceneChange`）添加光标控制
