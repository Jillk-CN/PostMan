# Design: 修复进入游戏场景后鼠标未隐藏的问题

## 根本原因

`TitleUIManager`（`DontDestroyOnLoad` 单例）在 `Start()` 中调用了 `GameInputManager.Instance.ShowCursor()`，使标题场景内鼠标可见。

当玩家点击 Start 按钮时，`OnStartGame()` 直接调用 `GameSceneManager.Instance.SwitchScenes(...)` 完成场景切换，但没有在此之前调用 `HideCursor()`。因为 `GameInputManager.Init()` 中的 `HideCursor()` 只在单例首次初始化时执行一次，此后不会重新触发，所以游戏场景加载完成后光标依然处于可见状态。

## 修复方案

### 修改文件

`Assets/Scripts/UI/Title/TitleUIManager.cs` — 仅修改 `OnStartGame()` 方法。

### 具体改动

```
OnStartGame()
  1. 调用 GameInputManager.Instance.HideCursor()    ← 新增
  2. 调用 GameSceneManager.Instance.SwitchScenes(scenesToLoad, scenesToUnload)  ← 已有，不变
```

修改后代码：

```csharp
private void OnStartGame()
{
    GameInputManager.Instance.HideCursor(); // 进入游戏前隐藏鼠标
    GameSceneManager.Instance.SwitchScenes(scenesToLoad, scenesToUnload);
}
```

### 不需要修改其他文件。

## 数据流

```
玩家点击 Start
  → OnStartGame()
    → GameInputManager.Instance.HideCursor()
        → Cursor.lockState = CursorLockMode.Locked
        → Cursor.visible = false
    → GameSceneManager.Instance.SwitchScenes(...)
        → 异步加载游戏场景
        → 卸载标题场景
  → 游戏场景运行，鼠标已被锁定隐藏 ✓
```

## 边界情况

| 情况 | 行为 |
|---|---|
| 玩家从标题场景打开设置后返回再点击 Start | `OnStartGame` 隐藏光标后再切换场景，正常 |
| `GameInputManager` 尚未初始化 | 不可能——`GameInputManager.Init()` 先于任何 UI 的 `Start()` 执行（均为 `DontDestroyOnLoad` 单例） |
| 游戏场景内有脚本再次调用 `ShowCursor()` | 不受影响——本修复仅确保进入场景时光标的初始状态正确 |
