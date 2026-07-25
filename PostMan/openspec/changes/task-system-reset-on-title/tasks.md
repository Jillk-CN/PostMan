# Tasks: Task System Reset on Return to Title

## Implementation

- [x] **Modify `PausePanel.cs`**
  - File: `Assets/Scripts/UI/PausePanel.cs`
  - Ensure `using System;` is present (needed for `Action`)
  - Add static event declaration in the public section (before or after `ReturnToTitleEvent`):
    ```csharp
    /// <summary>点击"返回主菜单"按钮时触发，在场景切换前发布，供各系统执行清理。</summary>
    public static event Action OnTitleTrigger;
    ```
  - In `OnReturnToTitle()`, add before `ReturnToTitleEvent.Invoke()`:
    ```csharp
    OnTitleTrigger?.Invoke();
    ```

- [x] **Modify `TaskManager.cs`**
  - File: `Assets/Scripts/TaskSystem/Runtime/TaskManager.cs`
  - Add `ResetAll()` public method:
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
  - In `Awake` (or `Init()` if `MonoSingleton` override is used), add subscription:
    ```csharp
    PausePanel.OnTitleTrigger += ResetAll;
    ```

- [x] **Modify `TaskUI.cs`**
  - File: `Assets/Scripts/TaskSystem/UI/TaskUI.cs`
  - Add `ForceHide()` public method:
    ```csharp
    /// <summary>
    /// 立即将任务面板移出屏幕（无动画），并重置可见状态标志。
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
  - In `OnEnable`, add: `PausePanel.OnTitleTrigger += ForceHide;`
  - In `OnDisable`, add: `PausePanel.OnTitleTrigger -= ForceHide;`

## Unity Editor Setup

- [ ] **Verify all three scripts compile** — open Unity Editor, confirm no compiler errors
- [ ] **Test the flow in Play mode:**
  - Start a task to get the task UI visible
  - Open pause panel (ESC)
  - Click the Title button
  - Confirm task UI snaps off-screen before the scene switches
  - After returning to title and starting a new game, confirm task system is in a clean state (no stale tasks)

## Verification Checklist

- [ ] `OnTitleTrigger` event exists on `PausePanel` and fires when Title button is clicked
- [ ] `TaskManager._activeTasks` is empty after `ResetAll()` is called
- [ ] `TaskUI` panel is off-screen (anchoredPosition.x == -width) after `ForceHide()` is called
- [ ] Any in-progress slide coroutine is stopped by `ForceHide()` — no resume after scene switch
- [ ] `_isVisible` is `false` after `ForceHide()`
- [ ] Starting a new game after returning to title does not show stale task data in the UI
- [ ] Clicking Resume or Settings from the pause panel does NOT fire `OnTitleTrigger`
- [ ] `TaskUI.OnDisable` correctly unsubscribes — `ForceHide` not called if `TaskUI` is disabled
