# Tasks: Task-Triggered BGM Switcher

## Implementation

- [ ] **Create `TaskBGMSwitcher.cs`**
  - File: `Assets/Scripts/BGM/TaskBGMSwitcher.cs`
  - Add `using` directives: `PostMan.AudioSystem`, `UnityEngine`
  - Declare class in global namespace (no `namespace` block), inheriting `MonoBehaviour`
  - Add XML `/// <summary>` doc comment (Simplified Chinese)
  - Add Inspector fields with `[Header]` and `[Tooltip]` groups:
    - `[Header("Task 触发配置")]` → `[SerializeField] TaskSO targetTask`
    - `[Header("Stop 参数")]` → `stopFadeOut (bool, default true)`, `stopFadeOutDuration (float, default 0.5f)`
    - `[Header("Play 参数")]` → `newClip (AudioClip)`, `loop (bool, default true)`, `playFadeIn (bool, default true)`, `playFadeInDuration (float, default 1f)`, `volume (float, default 1f)`
  - Implement `OnEnable` / `OnDisable` to subscribe/unsubscribe `HandleTaskStarted` to `TaskEventBus.OnTaskStarted`
  - Implement `HandleTaskStarted(TaskRuntimeData data)`:
    - Null guard: if `targetTask == null` or `newClip == null`, log error and return
    - Index check: if `data.TaskIndex != targetTask.taskIndex`, return
    - Call `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)`
    - Call `AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)`

## Unity Editor Setup

- [ ] **Verify script compiles** — open Unity Editor, confirm no compiler errors in the Console
- [ ] **Create a test GameObject** in the scene and attach `TaskBGMSwitcher`
- [ ] **Wire Inspector references:**
  - Assign a `TaskSO` asset to `Target Task`
  - Assign an `AudioClip` to `New Clip`
  - Adjust Stop/Play parameters as desired
- [ ] **Test the transition** — trigger the target task in Play mode and confirm:
  1. Current BGM fades out (or stops immediately if `stopFadeOut` is false)
  2. The new clip starts playing on the BGM track with the configured fade/loop/volume

## Verification Checklist

- [ ] Component appears in `Add Component` menu under no namespace (search "TaskBGMSwitcher")
- [ ] All Inspector fields are visible and editable in the Inspector
- [ ] Null guard logs a clear error if `targetTask` or `newClip` is unassigned
- [ ] Triggering a non-matching Task does NOT switch the BGM
- [ ] Triggering the matching Task switches the BGM with correct fade behaviour
- [ ] Unsubscription in `OnDisable` prevents callbacks after the component is disabled/destroyed
