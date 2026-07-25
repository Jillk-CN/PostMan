# Design: SceneBGMSwitcher sceneOrder Branch

## Overview

`Assets/Scripts/BGM/SceneBGMSwitcher.cs` is modified to add a second, independent trigger path: a `sceneOrder`-based branch that uses `SceneInitializer.Register` to receive the per-scene-load callback and plays `newClip2` when the order matches.

## Modified Component

### `SceneBGMSwitcher : MonoBehaviour`

**File:** `Assets/Scripts/BGM/SceneBGMSwitcher.cs`

The `using PostMan.Scene;` directive is already present in the file.

#### New Inspector Fields

Added after the existing `[Header("场景触发配置")]` block:

```
[Header("SceneOrder 触发配置")]
[SerializeField] bool  enableSceneOrderTrigger  // 启用 sceneOrder 分支 (default: false)
[SerializeField] int   targetSceneOrder         // 目标 sceneOrder 值（与 SceneInitializer 中的 sceneOrder 一致）
[SerializeField] bool  triggerOnce2             // 是否只触发一次 (default: true)
```

Added inside the existing `[Header("Play 参数")]` block, after the already-present `newClip2` field:

```
[Tooltip on newClip2]  // 仅 SceneOrder 分支使用；必须在启用 enableSceneOrderTrigger 时赋值
```

(The `newClip2` field itself already exists in the file — only the `[Tooltip]` and `/// <summary>` are added.)

All other Stop/Play parameters (`stopFadeOut`, `stopFadeOutDuration`, `loop`, `playFadeIn`, `playFadeInDuration`, `volume`) are **shared** between both branches.

#### New Runtime State

```
private bool _triggered2 = false;   // triggerOnce2 guard for the sceneOrder branch
```

#### Lifecycle Changes

`OnEnable` — add `SceneInitializer.Instance.Register(HandleSceneOrder)` after the existing subscription.

`OnDisable` — no change needed; `SceneInitializer.initOperation` is cleared after each fire; the next `OnEnable` re-registers.

#### New Callback

```
HandleSceneOrder(int sceneOrder, string sceneName):
  if !enableSceneOrderTrigger → return
  if newClip2 == null → LogError with this context, return
  if triggerOnce2 && _triggered2 → return
  if sceneOrder != targetSceneOrder → return
  _triggered2 = true
  AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)
  AudioManager.Instance.Play(AudioTrackId.BGM, newClip2, loop, playFadeIn, playFadeInDuration, volume)
```

The `enableSceneOrderTrigger` toggle lets designers leave the branch inactive (default) without having to assign `newClip2`, avoiding spurious LogError spam.

## Dependencies & Integration

| Dependency | Already Present | How Used |
|---|---|---|
| `SceneInitializer` | Yes (`using PostMan.Scene`) | `Instance.Register(HandleSceneOrder)` |
| `SceneInitHandler` delegate | Yes (same namespace) | Callback signature `(int, string)` |
| `AudioManager` | Yes | `Instance.Stop()` + `Instance.Play()` |
| `AudioTrackId` | Yes | `AudioTrackId.BGM` |

## Execution Flow

```
GameSceneManager.SwitchScenes() completes
  → OnSceneSwitchCompleted fires
    → SceneInitializer.OnSceneLoad increments sceneOrder, invokes initOperation
      → SceneBGMSwitcher.HandleSceneOrder(sceneOrder, sceneName)
        → enableSceneOrderTrigger? → yes
        → newClip2 null guard passes
        → triggerOnce2 guard: not yet triggered
        → sceneOrder == targetSceneOrder? → yes
          → _triggered2 = true
          → AudioManager.Stop(BGM, fadeOut, duration)
          → AudioManager.Play(BGM, newClip2, loop, fadeIn, duration, volume)
```

Note: `SceneInitializer.OnSceneLoad` also fires on the same `OnSceneSwitchCompleted` event as `SceneBGMSwitcher.HandleSceneSwitchCompleted`. Execution order between the two is not guaranteed; they are independent paths.

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/BGM/SceneBGMSwitcher.cs` | **Modify** — add `[Header("SceneOrder 触发配置")]` block, add `_triggered2` field, add `Register` call in `OnEnable`, add `HandleSceneOrder` method, annotate `newClip2` |

No other files need modification.
