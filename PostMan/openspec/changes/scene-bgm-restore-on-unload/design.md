# Design: Scene BGM Restore on Unload

## Overview

`Assets/Scripts/BGM/SceneBGMSwitcher.cs` is modified to add an opt-in restore path. When `restoreOnUnload` is enabled, the component snapshots the pre-switch BGM on load and restores it the next time `OnSceneSwitchCompleted` fires without `targetSceneAddress` in the loaded list.

## Modified Component

### `SceneBGMSwitcher : MonoBehaviour`

**File:** `Assets/Scripts/BGM/SceneBGMSwitcher.cs`

#### New Inspector Field

Added to the `[Header("场景触发配置")]` block, after `triggerOnce`:

```
[SerializeField] bool restoreOnUnload   // 卸载目标场景时还原之前的 BGM (default: false)
```

No new header section — it belongs conceptually to the scene-address trigger configuration.

#### New Runtime State

```
private AudioClip _previousClip;        // 加载 targetSceneAddress 前正在播放的 BGM 快照
```

#### Modified Load Path (`HandleSceneSwitchCompleted` — load branch)

Before the existing `AudioManager.Instance.Stop` call, insert:

```
// 快照当前 BGM，供卸载时还原（仅 restoreOnUnload 启用时有意义）
_previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM);
```

#### New Unload Detection (same `HandleSceneSwitchCompleted` handler)

At the top of `HandleSceneSwitchCompleted`, after the `targetSceneAddress` guard and before the `triggerOnce` guard, add:

```
// 卸载还原分支：已切换过 BGM 且本次切换不含目标场景 → 目标场景已被卸载
if (restoreOnUnload && _triggered && !loadedScenes.Contains(targetSceneAddress))
{
    _triggered = false;   // 重置，允许下次重新加载时再次触发
    AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
    if (_previousClip != null)
        AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f);
    return;
}
```

Resetting `_triggered = false` here is intentional: it allows the load path to fire again if the scene is reloaded later (even with `triggerOnce = true`, because the "once" refers to once per load/unload cycle).

#### Full Updated Logic Flow for `HandleSceneSwitchCompleted`

```
HandleSceneSwitchCompleted(loadedScenes):
  1. newClip null guard (with commented-out LogError — matches current file style)
  2. targetSceneAddress empty guard → LogError, return
  3. [NEW] if restoreOnUnload && _triggered && !loadedScenes.Contains(targetSceneAddress):
       → _triggered = false
       → Stop(BGM, stopFadeOut, stopFadeOutDuration)
       → if _previousClip != null: Play(BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f)
       → return
  4. triggerOnce && _triggered → return
  5. !loadedScenes.Contains(targetSceneAddress) → return
  6. _previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM)   [NEW]
  7. _triggered = true
  8. Stop(BGM, stopFadeOut, stopFadeOutDuration)
  9. Play(BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)
```

## Dependencies & Integration

| Dependency | Already Present | How Used |
|---|---|---|
| `AudioManager.GetClip` | Yes | Snapshot BGM before Stop |
| `AudioTrackId.BGM` | Yes | Track identifier |
| `ColliderBGMSwitcher` | Reference only | Same snapshot+restore pattern |

## Execution Flow

```
[Load] GameSceneManager.SwitchScenes({targetSceneAddress}, ...)
  → OnSceneSwitchCompleted([targetSceneAddress])
    → HandleSceneSwitchCompleted: unload branch skipped (_triggered=false)
    → _previousClip = GetClip(BGM)
    → _triggered = true
    → Stop + Play(newClip)

[Unload] GameSceneManager.SwitchScenes({otherScene}, {targetSceneAddress})
  → OnSceneSwitchCompleted([otherScene])
    → HandleSceneSwitchCompleted: unload branch fires (restoreOnUnload=true, _triggered=true, targetSceneAddress not in list)
    → _triggered = false
    → Stop + Play(_previousClip)  [or just Stop if _previousClip == null]
```

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/BGM/SceneBGMSwitcher.cs` | **Modify** — add `restoreOnUnload` field, `_previousClip` field, snapshot line in load path, unload detection branch |

No other files need modification.
