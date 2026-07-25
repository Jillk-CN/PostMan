# Tasks: SceneBGMSwitcher sceneOrder Branch

## Implementation

- [x] **Modify `SceneBGMSwitcher.cs`**
  - File: `Assets/Scripts/BGM/SceneBGMSwitcher.cs`
  - Confirm `using PostMan.Scene;` is already present (it is)
  - Add a new `[Header("SceneOrder 触发配置")]` Inspector block after the existing `[Header("场景触发配置")]` block:
    - `enableSceneOrderTrigger (bool, default false)` with `[Tooltip]` — 启用后才监听 SceneInitializer 的 sceneOrder 回调
    - `targetSceneOrder (int, default 0)` with `[Tooltip]` — 目标 sceneOrder 值，与 SceneInitializer.SceneOrder 对应
    - `triggerOnce2 (bool, default true)` with `[Tooltip]` — 是否只触发一次
  - Annotate the existing `newClip2` field with `/// <summary>` and `[Tooltip]` (仅 SceneOrder 分支使用)
  - Add private `bool _triggered2 = false` to the runtime state section
  - In `OnEnable`, add: `SceneInitializer.Instance.Register(HandleSceneOrder);`
  - Add private method `HandleSceneOrder(int sceneOrder, string sceneName)`:
    - `if (!enableSceneOrderTrigger) return;`
    - `if (newClip2 == null)` → `Debug.LogError(...)` with `this` context, return
    - `if (triggerOnce2 && _triggered2) return;`
    - `if (sceneOrder != targetSceneOrder) return;`
    - `_triggered2 = true;`
    - `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);`
    - `AudioManager.Instance.Play(AudioTrackId.BGM, newClip2, loop, playFadeIn, playFadeInDuration, volume);`

## Unity Editor Setup

- [ ] **Verify script compiles** — open Unity Editor, confirm no compiler errors in the Console
- [ ] **Select the GameObject** that has `SceneBGMSwitcher` attached
- [ ] **Enable the sceneOrder branch:**
  - Check `Enable Scene Order Trigger`
  - Set `Target Scene Order` to the sceneOrder value at which this BGM should play (check `SceneInitializer.SceneOrder` in Play mode or read the Init component on the scene to determine the value)
  - Assign an `AudioClip` to `New Clip 2`
  - Adjust `Trigger Once 2` as needed

## Verification Checklist

- [ ] Script compiles with no errors
- [ ] `Enable Scene Order Trigger` unchecked: `HandleSceneOrder` returns immediately; no BGM change and no LogError even if `newClip2` is unassigned
- [ ] `Enable Scene Order Trigger` checked, `newClip2` unassigned: LogError fires; no BGM change
- [ ] Entering the matching `sceneOrder` plays `newClip2` with correct fade behaviour
- [ ] Non-matching `sceneOrder` does NOT trigger the switch
- [ ] `triggerOnce2 = true`: the same `sceneOrder` firing a second time does NOT re-trigger
- [ ] `triggerOnce2 = false`: the same `sceneOrder` re-triggers on each re-registration
- [ ] The existing address-based path (`newClip`, `targetSceneAddress`) still works independently
