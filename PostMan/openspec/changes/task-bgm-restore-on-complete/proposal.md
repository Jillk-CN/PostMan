# Proposal: TaskBGMSwitcher Restore on Task Complete

## Problem

`TaskBGMSwitcher` switches the BGM when a target task starts, but it never restores the previous BGM when that task is completed. The switched BGM keeps playing indefinitely past the task's natural end. `SceneBGMSwitcher` gained a `restoreOnUnload` feature that snapshots and restores the pre-switch clip using the same AudioManager API. `TaskBGMSwitcher` needs the equivalent: restore the previous BGM when the task completes.

## Solution

Add an opt-in `restoreOnTaskComplete` bool to `TaskBGMSwitcher`. When enabled, the component snapshots the currently-playing BGM (via `AudioManager.Instance.GetClip`) immediately before executing the Start-triggered Stop+Play. It then subscribes to `TaskEventBus.OnTaskCompleted`, and when the matching task completes, it restores the snapshot — stopping the current BGM and replaying the previous clip.

## Key Decisions

- **Opt-in via `restoreOnTaskComplete`** — off by default; existing behaviour is unchanged when unchecked.
- **`TaskEventBus.OnTaskCompleted`** — the natural counterpart to `OnTaskStarted`; already exists, uses the same `TaskRuntimeData` payload with `data.Definition.taskIndex` for matching.
- **Snapshot at switch time** — `_previousClip` is captured immediately before `AudioManager.Stop` in `HandleTaskStarted`, identical to the `SceneBGMSwitcher` and `ColliderBGMSwitcher` pattern.
- **Subscribe in `OnEnable` / unsubscribe in `OnDisable`** — both `OnTaskStarted` and `OnTaskCompleted` are managed symmetrically, consistent with the existing lifecycle pattern in `TaskBGMSwitcher`.
- **`_triggered` guard** — a flag ensures the restore only fires if a BGM switch actually happened (i.e., `HandleTaskStarted` succeeded). Prevents spurious restores if the task completes without a prior switch.

## Non-goals

- Does not handle `OnTaskFailed` as a restore trigger.
- Does not support multiple restore cycles (once restored the component is dormant unless `_triggered` is reset — which doesn't happen by default).
- Does not add new `AudioManager` or `TaskEventBus` methods.
- Does not create any UI or visual feedback.
