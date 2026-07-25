# Proposal: Task System Reset on Return to Title

## Problem

`TaskManager` is a `DontDestroyOnLoad` singleton whose `_activeTasks` dictionary is never cleared. `TaskUI` has no forced-hide path. When the player uses the pause panel's Title button to return to the main menu, both systems carry stale state into the next session: previously started/completed tasks remain in the dictionary, and the task UI panel may still be visible on screen. Starting a new game from the title after this will produce incorrect task behaviour and a visible UI artifact.

## Solution

Three changes working together:

1. **`PausePanel`** — publish a new static C# event `OnTitleTrigger` from `OnReturnToTitle()`, fired before the scene switch. This is a decoupled broadcast that any system can subscribe to without depending on `PausePanel` directly.

2. **`TaskManager`** — add a `ResetAll()` public method that clears `_activeTasks` and publishes no events (silent reset). Subscribe to `PausePanel.OnTitleTrigger` in `OnEnable`/`OnDisable` (or as a permanent subscription in `Awake` since it is `DontDestroyOnLoad`) to call `ResetAll()` automatically.

3. **`TaskUI`** — add a `ForceHide()` method that immediately snaps the panel off-screen (no animation coroutine, since the scene is about to change). Subscribe to `PausePanel.OnTitleTrigger` in `OnEnable`/`OnDisable` and call `ForceHide()`.

## Key Decisions

- **Static C# event on `PausePanel`** — consistent with `GameSceneManager.OnSceneSwitchCompleted` and `TaskEventBus` patterns already used in the project. Lower coupling than direct method calls; any future subscriber (BGM, save system, etc.) can hook in without touching `PausePanel`.
- **`ResetAll()` is silent** — no events are published during reset. Task subscribers should not react to cleanup as if tasks were completing or failing; the scene switch already tears down the game state.
- **`ForceHide()` snaps to off-screen** — no slide animation during reset; the scene is changing and a visible in-progress slide would look wrong. Position is set to the same off-screen value used by `Awake`.
- **`TaskManager` subscribes in `Awake`** — it is `DontDestroyOnLoad` and lives for the entire session; a permanent subscription set up once in `Awake` is cleaner than `OnEnable`/`OnDisable` toggling.
- **`TaskUI` subscribes in `OnEnable`/`OnDisable`** — it is a normal scene object and follows the standard Unity subscription lifecycle.

## Non-goals

- Does not reset `SceneInitializer.sceneOrder` (separate concern).
- Does not save or restore task progress across sessions.
- Does not reset BGM switchers or any other subscriber — each system is responsible for its own cleanup.
- Does not modify `TaskEventBus` publish methods.
