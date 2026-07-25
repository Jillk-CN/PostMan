# Proposal: Scene BGM Restore on Unload

## Problem

`SceneBGMSwitcher` plays a new BGM when `targetSceneAddress` is loaded, but it never restores the previous BGM when that scene is later unloaded. Once the player leaves the scene, the new BGM keeps playing indefinitely. `ColliderBGMSwitcher` solves the equivalent problem for trigger zones by snapshotting the pre-enter BGM and restoring it on `OnTriggerExit`. `SceneBGMSwitcher` has no equivalent restore path.

## Solution

Add an opt-in `restoreOnUnload` bool to `SceneBGMSwitcher`. When enabled, the component snapshots the currently-playing BGM clip (via `AudioManager.Instance.GetClip`) immediately before executing the address-based Stop+Play. Then, when `OnSceneSwitchCompleted` fires again and the loaded-scene list no longer contains `targetSceneAddress`, it restores the snapshot — matching the pattern used by `ColliderBGMSwitcher`.

Detection of "unloaded" is done entirely inside the existing `HandleSceneSwitchCompleted` handler: if `_triggered` is true (scene was previously loaded and BGM was switched) and `loadedScenes` does not contain `targetSceneAddress`, the scene has been unloaded and the restore fires. No changes to `GameSceneManager` are needed.

## Key Decisions

- **Opt-in via `restoreOnUnload`** — off by default; designers who don't need restore are unaffected.
- **Snapshot at switch time** — `_previousClip` is captured immediately before `AudioManager.Stop` in the load path, identical to `ColliderBGMSwitcher`. The snapshot is a best-effort capture; if no BGM was playing, it is null and only Stop is called on restore.
- **Reuse existing `HandleSceneSwitchCompleted`** — no new event subscription or second handler needed; the unload detection is a branch in the same callback.
- **`_triggered` as state guard** — the load path sets `_triggered = true` and captures the snapshot. The restore path checks `_triggered` to confirm a switch actually happened before restoring.
- **No changes to `GameSceneManager`** — avoids modifying a shared, stable system; works with the existing `OnSceneSwitchCompleted` payload.

## Non-goals

- Does not restore after the `sceneOrder` branch (`newClip2`).
- Does not support multiple unload/reload cycles when `triggerOnce = true` (once triggered and restored, the component is dormant).
- Does not add new `AudioManager` or `GameSceneManager` methods.
- Does not create any UI or visual feedback.
