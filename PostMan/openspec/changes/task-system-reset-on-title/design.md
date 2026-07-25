# Design: Task System Reset on Return to Title

## Overview

Three files are modified. `PausePanel` gains a static event and fires it in the Title button handler. `TaskManager` gains a `ResetAll()` method and subscribes to the event. `TaskUI` gains a `ForceHide()` method and subscribes to the event.

---

## 1. `PausePanel` — `Assets/Scripts/UI/PausePanel.cs`

### New Static Event

Add to the public section of the class, before `ReturnToTitleEvent`:

```csharp
/// <summary>
/// 点击"返回主菜单"按钮时触发的静态事件。
/// 在场景切换前发布，供各系统执行清理操作。
/// </summary>
public static event Action OnTitleTrigger;
```

Requires `using System;` — check if already present; add if missing.

### Fire the Event

In `OnReturnToTitle()`, add the invocation **before** `ReturnToTitleEvent.Invoke()`:

```csharp
OnTitleTrigger?.Invoke();
```

---

## 2. `TaskManager` — `Assets/Scripts/TaskSystem/Runtime/TaskManager.cs`

### New `ResetAll()` Method

```csharp
/// <summary>
/// 清空所有运行时任务状态，恢复到初始状态。
/// 不发布任何任务事件；仅用于返回主菜单等全局重置场景。
/// </summary>
public void ResetAll()
{
    _activeTasks.Clear();
}
```

### Subscribe to `OnTitleTrigger`

In `Awake` (or the `Init()` override if `MonoSingleton` uses one), add:

```csharp
PausePanel.OnTitleTrigger += ResetAll;
```

No unsubscribe is needed because `TaskManager` is `DontDestroyOnLoad` and lives for the entire application session.

**Note:** If `TaskManager` overrides `Init()` from `MonoSingleton`, put the subscription there. If it uses `Awake`, put it in `Awake`. Check the actual file before implementing.

---

## 3. `TaskUI` — `Assets/Scripts/TaskSystem/UI/TaskUI.cs`

### New `ForceHide()` Method

```csharp
/// <summary>
/// 立即将任务面板移出屏幕（无动画），并重置可见状态。
/// 用于场景切换前的快速清理。
/// </summary>
public void ForceHide()
{
    if (_slideCoroutine != null)
    {
        StopCoroutine(_slideCoroutine);
        _slideCoroutine = null;
    }
    taskPanel.anchoredPosition = new Vector2(-taskPanel.rect.width, taskPanel.anchoredPosition.y);
    _isVisible = false;
}
```

### Subscribe to `OnTitleTrigger`

In `OnEnable`, add:

```csharp
PausePanel.OnTitleTrigger += ForceHide;
```

In `OnDisable`, add:

```csharp
PausePanel.OnTitleTrigger -= ForceHide;
```

---

## Execution Flow

```
Player clicks Title button
  → PausePanel.OnReturnToTitle()
    → OnTitleTrigger?.Invoke()
      → TaskManager.ResetAll()       : _activeTasks.Clear()
      → TaskUI.ForceHide()           : stop coroutine, snap off-screen, _isVisible = false
    → ReturnToTitleEvent.Invoke()    (existing UnityEvent — unchanged)
    → GameSceneManager.Instance.SwitchScenes(...)
```

---

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/UI/PausePanel.cs` | **Modify** — add `OnTitleTrigger` static event, fire it in `OnReturnToTitle` |
| `Assets/Scripts/TaskSystem/Runtime/TaskManager.cs` | **Modify** — add `ResetAll()`, subscribe in `Awake`/`Init` |
| `Assets/Scripts/TaskSystem/UI/TaskUI.cs` | **Modify** — add `ForceHide()`, subscribe/unsubscribe in `OnEnable`/`OnDisable` |
