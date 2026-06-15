# Design: Fix InputSource State Corruption on Scene Reload

## Overview

Two small changes to existing files restore input sources to a clean state every time a new game session begins.

---

## Change 1 — `GameInputManager.cs`

**File:** `Assets/Scripts/InputManagement/GameInputManager.cs`

Add a public `ResetToGameDefaults()` method that brings every `IInputSource` to the correct initial state for a gameplay session:

- **Player inputs enabled:** `PlayerMotionInputSource`, `PlayerSightInputSource`, `PlayerInteractInputSource`
- **UI/meta inputs disabled at start:** `PauseInputSource` is left *enabled* (player should be able to pause immediately), while `UIReturnInputSource`, `VHSToggleInputSource`, `DialogueInputSource` are *disabled* (they are enabled on demand by their respective panels/systems)

```csharp
/// <summary>
/// 将所有输入源重置为游戏场景默认状态。
/// 在每次加载游戏场景后调用，防止上一次游戏会话中的启用/禁用状态污染新会话。
/// </summary>
public void ResetToGameDefaults()
{
    // 玩家核心输入：默认全部启用
    SetInputSystemSource<PlayerMotionInputSource>(true);
    SetInputSystemSource<PlayerSightInputSource>(true);
    SetInputSystemSource<PlayerInteractInputSource>(true);

    // 暂停键：游戏开始即可用
    SetInputSystemSource<PauseInputSource>(true);

    // 按需输入源：由各自系统在需要时启用，此处确保初始为关闭
    SetInputSystemSource<UIReturnInputSource>(false);
    SetInputSystemSource<VHSToggleInputSource>(false);
    SetInputSystemSource<DialogueInputSource>(false);
}
```

No new fields or lifecycle hooks are needed. `SetInputSystemSource<T>` already does a null-safe lookup, so sources that are not present (e.g., in a scene that doesn't include all sources) are silently skipped.

---

## Change 2 — `GameSceneManager.cs`

**File:** `Assets/Scripts/SceneManagement/GameSceneManager.cs`

Call `GameInputManager.Instance?.ResetToGameDefaults()` after all target scenes have finished loading (阶段 1 完成后、传送玩家前). This is the correct moment: all new scene objects exist and are initialized, but old scenes have not yet been unloaded.

```csharp
// ── 阶段 1.5a：重置输入源状态 ──
GameInputManager.Instance?.ResetToGameDefaults();

// ── 阶段 1.5b：传送玩家到出生点 ──（已有逻辑）
if (spawnPoint.HasValue)
    TeleportPlayer(spawnPoint.Value);
```

The `?.` null-conditional ensures no crash in scenes where `GameInputManager` is absent (e.g., pure cutscene or loading scenes).

---

## Call-site Placement

```
SwitchScenes()
  ├── 阶段 1: await 并行加载所有目标场景          ← 已有
  ├── 阶段 1.5a: ResetToGameDefaults()            ← 新增
  ├── 阶段 1.5b: TeleportPlayer()                 ← 已有
  └── 阶段 2: 串行卸载旧场景                       ← 已有
```

Placing the reset *before* unload means the old (Title) scene's UI objects are still alive, but they will be destroyed moments later. This is intentional — the reset only affects `GameInputManager`'s internal source state, not any UI component lifecycles.

---

## Why Not Reset in `GameInputManager.OnEnable` or `Init`?

`Init()` runs only once (when the singleton is first found by `MonoSingleton<T>`). Subsequent scene reloads never trigger `Init()` again because the object survives via `DontDestroyOnLoad`. `OnEnable` is equally unsuitable since the GameObject is never disabled. The explicit call from `GameSceneManager` is the only reliable hook point.

---

## Affected Files Summary

| File | Change |
|------|--------|
| `Assets/Scripts/InputManagement/GameInputManager.cs` | Add `ResetToGameDefaults()` method |
| `Assets/Scripts/SceneManagement/GameSceneManager.cs` | Call `ResetToGameDefaults()` after scene load, before teleport |
