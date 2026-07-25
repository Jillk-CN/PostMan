# Design: TaskBGMSwitcher Restore on Task Complete

## Overview

`Assets/Scripts/BGM/TaskBGMSwitcher.cs` is modified to add an opt-in restore path. When `restoreOnTaskComplete` is enabled, the component snapshots the pre-switch BGM on task start and restores it when `TaskEventBus.OnTaskCompleted` fires for the matching task.

## Modified Component

### `TaskBGMSwitcher : MonoBehaviour`

**File:** `Assets/Scripts/BGM/TaskBGMSwitcher.cs`

#### New Inspector Field

Added to the `[Header("Task 触发配置")]` block, after `targetTask`:

```
[SerializeField] bool restoreOnTaskComplete   // 任务完成时还原之前的 BGM (default: false)
```

#### New Runtime State

```
private bool      _triggered     = false;   // 切换保护标志；任务启动成功后置 true
private AudioClip _previousClip;            // 任务启动前正在播放的 BGM 快照，完成时用于还原
```

#### Lifecycle Changes

`OnEnable` — add `TaskEventBus.OnTaskCompleted += HandleTaskCompleted`
`OnDisable` — add `TaskEventBus.OnTaskCompleted -= HandleTaskCompleted`

#### Modified Start Handler (`HandleTaskStarted`)

Before the existing `AudioManager.Instance.Stop` call, insert:

```csharp
_triggered    = true;
_previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM);
```

#### New Complete Handler

```
HandleTaskCompleted(TaskRuntimeData data):
  if !restoreOnTaskComplete → return
  if !_triggered → return                              // switch never happened
  if data.Definition.taskIndex != targetTask.taskIndex → return
  _triggered = false
  AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)
  if _previousClip != null:
      AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f)
```

`_triggered` is reset to `false` after restore so the component is idempotent if the event fires multiple times.

## Dependencies & Integration

| Dependency | Already Present | How Used |
|---|---|---|
| `TaskEventBus.OnTaskCompleted` | Yes | Subscribe/unsubscribe for restore |
| `TaskRuntimeData.Definition.taskIndex` | Yes | Match against `targetTask.taskIndex` |
| `AudioManager.GetClip` | Yes | Snapshot BGM before Stop |
| `AudioTrackId.BGM` | Yes | Track identifier |

## Execution Flow

```
[Task Start] TaskManager.StartTask(targetTask)
  → TaskEventBus.PublishStarted(data)
    → HandleTaskStarted(data): taskIndex matches
      → _triggered = true
      → _previousClip = GetClip(BGM)
      → Stop + Play(newClip)

[Task Complete] TaskManager advances task to completion
  → TaskEventBus.PublishCompleted(data)
    → HandleTaskCompleted(data): restoreOnTaskComplete=true, _triggered=true, taskIndex matches
      → _triggered = false
      → Stop + Play(_previousClip)  [or just Stop if _previousClip == null]
```

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/BGM/TaskBGMSwitcher.cs` | **Modify** — add `restoreOnTaskComplete` field, `_triggered` + `_previousClip` runtime fields, snapshot + flag in `HandleTaskStarted`, subscribe/unsubscribe `OnTaskCompleted`, add `HandleTaskCompleted` method |

No other files need modification.
