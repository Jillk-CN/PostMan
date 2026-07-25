# Design: Interactable BGM Switcher

## Overview

A single new `MonoBehaviour` — `InteractableBGMSwitcher` — is created at `Assets/Scripts/BGM/InteractableBGMSwitcher.cs`. It implements `PostMan.Player.IInteractable` and uses `TaskEventBus.OnTaskCompleted` for the optional restore path.

## Component Design

### `InteractableBGMSwitcher : MonoBehaviour, IInteractable`

**File:** `Assets/Scripts/BGM/InteractableBGMSwitcher.cs`
**Namespace:** global (consistent with all BGM switchers and concrete interactable objects)

#### Using Directives

```csharp
using PostMan.AudioSystem;
using PostMan.Player;
using UnityEngine;
```

#### Inspector Fields

```
[Header("交互配置")]
[SerializeField] bool  canInteract   // IInteractable 实现；默认可交互 (default: true)
[SerializeField] int   priority      // IInteractable 优先级 (default: 0)
[SerializeField] bool  interactOnce  // 只允许触发一次交互 (default: true)

[Header("Task 还原配置")]
[SerializeField] bool   restoreOnTaskComplete   // 任务完成时还原之前的 BGM (default: false)
[SerializeField] TaskSO targetTask              // 触发还原的目标任务

[Header("Stop 参数")]
[SerializeField] bool  stopFadeOut          (default: true)
[SerializeField] float stopFadeOutDuration  (default: 0.5f)

[Header("Play 参数")]
[SerializeField] AudioClip newClip
[SerializeField] bool  loop                 (default: true)
[SerializeField] bool  playFadeIn           (default: true)
[SerializeField] float playFadeInDuration   (default: 1f)
[SerializeField] float volume [Range(0,1)]  (default: 1f)
```

#### IInteractable Properties

```csharp
public bool CanInteract { get => canInteract; set => canInteract = value; }
public int  Priority    { get => priority;    set => priority    = value; }
```

#### Runtime State

```
private bool      _triggered    = false;   // interactOnce guard + restore entry gate
private AudioClip _previousClip;           // snapshot taken at interaction time
```

#### Lifecycle

- `OnEnable` — subscribe `HandleTaskCompleted` to `TaskEventBus.OnTaskCompleted`
- `OnDisable` — unsubscribe `HandleTaskCompleted` from `TaskEventBus.OnTaskCompleted`

#### `InteractWith(PlayerInteractor player)`

```
if newClip == null → LogError, return
if interactOnce && _triggered → return
_triggered    = true
_previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM)
AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)
AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)
```

#### `HandleTaskCompleted(TaskRuntimeData data)`

```
if !restoreOnTaskComplete → return
if !_triggered → return
if targetTask == null → return
if data.Definition.taskIndex != targetTask.taskIndex → return
_triggered = false
AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)
if _previousClip != null:
    AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f)
```

## Dependencies & Integration

| Dependency | Location | How Used |
|---|---|---|
| `IInteractable` | `Assets/Scripts/Player/Interaction/IInteractable.cs` | Interface implemented |
| `PlayerInteractor` | `Assets/Scripts/Player/Interaction/PlayerInteractor.cs` | Passed to `InteractWith`; not used |
| `AudioManager` | `Assets/Scripts/AudioSystem/AudioManager.cs` | `Stop` + `Play` + `GetClip` |
| `AudioTrackId` | `Assets/Scripts/AudioSystem/AudioTrackId.cs` | `AudioTrackId.BGM` |
| `TaskEventBus` | `Assets/Scripts/TaskSystem/Runtime/TaskEventBus.cs` | `OnTaskCompleted` |
| `TaskRuntimeData` | `Assets/Scripts/TaskSystem/Runtime/TaskRuntimeData.cs` | `data.Definition.taskIndex` |
| `TaskSO` | `Assets/Scripts/TaskSystem/Data/TaskSO.cs` | Inspector reference for restore target |

## Scene Setup

- Add `InteractableBGMSwitcher` as a component on any scene GameObject the player can look at.
- The same GameObject should have a `Collider` so `PlayerDetector` can raycast it.
- Optionally combine with `ISelectable` implementors (e.g. `OutlineVisual`) on the same object for highlight feedback — they operate independently.

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/BGM/InteractableBGMSwitcher.cs` | **Create** (new script, ~110 lines) |

No existing files need modification.
