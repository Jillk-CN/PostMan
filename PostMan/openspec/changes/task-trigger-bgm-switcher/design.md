# Design: Task-Triggered BGM Switcher

## Overview

A single new `MonoBehaviour` — `TaskBGMSwitcher` — is added to `Assets/Scripts/BGM/TaskBGMSwitcher.cs`. It wires `TaskEventBus.OnTaskStarted` to a two-step AudioManager call: Stop then Play on the BGM track.

## Component Design

### `TaskBGMSwitcher : MonoBehaviour`

**File:** `Assets/Scripts/BGM/TaskBGMSwitcher.cs`
**Namespace:** global (consistent with TaskSystem classes and BGMPlayer)

#### Inspector Fields

```
[Header("Task 触发配置")]
[SerializeField] TaskSO targetTask          // 监听的目标 Task

[Header("Stop 参数")]
[SerializeField] bool  stopFadeOut          // 停止时是否淡出 (default: true)
[SerializeField] float stopFadeOutDuration  // 淡出时长 (default: 0.5f)

[Header("Play 参数")]
[SerializeField] AudioClip newClip         // 要播放的新音乐
[SerializeField] bool  loop                // 是否循环 (default: true)
[SerializeField] bool  playFadeIn          // 播放时是否淡入 (default: true)
[SerializeField] float playFadeInDuration  // 淡入时长 (default: 1f)
[SerializeField] float volume              // 音量 (default: 1f)
```

**Note:** `trackId` is always `AudioTrackId.BGM` (hard-coded); this matches the requirement of operating specifically on the BGM track. No Inspector field is needed for it.

#### Lifecycle

- `OnEnable` — subscribe `HandleTaskStarted` to `TaskEventBus.OnTaskStarted`
- `OnDisable` — unsubscribe `HandleTaskStarted` from `TaskEventBus.OnTaskStarted`

#### Core Logic

```
HandleTaskStarted(TaskRuntimeData data):
  if data.TaskIndex != targetTask.taskIndex → return
  AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)
  AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)
```

A null guard on `targetTask` and `newClip` logs an error and returns early to prevent silent failures.

## Dependencies & Integration

| Dependency | Location | How Used |
|---|---|---|
| `AudioManager` | `Assets/Scripts/AudioSystem/AudioManager.cs` | `Instance.Stop()` + `Instance.Play()` |
| `AudioTrackId` | `Assets/Scripts/AudioSystem/AudioTrackId.cs` | `AudioTrackId.BGM` constant |
| `TaskEventBus` | `Assets/Scripts/TaskSystem/Runtime/TaskEventBus.cs` | `OnTaskStarted` event |
| `TaskRuntimeData` | `Assets/Scripts/TaskSystem/Runtime/TaskRuntimeData.cs` | Event payload, compare `TaskIndex` |
| `TaskSO` | `Assets/Scripts/TaskSystem/Data/TaskSO.cs` | Inspector reference for target task |
| `BGMPlayer` | `Assets/Scripts/BGM/BGMPlayer.cs` | Sibling file; no code dependency |

## Execution Flow

```
Task collider triggers → TaskManager.StartTask()
  → TaskEventBus.PublishStarted(data)
    → TaskBGMSwitcher.HandleTaskStarted(data)
      → taskIndex match? → yes
        → AudioManager.Stop(BGM, fadeOut, duration)
        → AudioManager.Play(BGM, clip, loop, fadeIn, duration, volume)
```

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/BGM/TaskBGMSwitcher.cs` | **Create** (new script, ~70 lines) |

No existing files need modification.
