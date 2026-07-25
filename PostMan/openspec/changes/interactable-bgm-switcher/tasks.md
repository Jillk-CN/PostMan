# Tasks: Interactable BGM Switcher

## Implementation

- [x] **Create `InteractableBGMSwitcher.cs`**
  - File: `Assets/Scripts/BGM/InteractableBGMSwitcher.cs`
  - Add `using` directives: `PostMan.AudioSystem`, `PostMan.Player`, `UnityEngine`
  - Declare class in global namespace, inheriting `MonoBehaviour` and implementing `IInteractable`
  - Add XML `/// <summary>` doc comment (Simplified Chinese)
  - Add Inspector fields with `[Header]` and `[Tooltip]`:
    - `[Header("交互配置")]`
      - `canInteract (bool, default true)` — IInteractable 实现；控制是否可被玩家交互
      - `priority (int, default 0)` — IInteractable 优先级，同一物体多个 IInteractable 时决定调用顺序
      - `interactOnce (bool, default true)` — 只允许触发一次，之后交互不再切换 BGM
    - `[Header("Task 还原配置")]`
      - `restoreOnTaskComplete (bool, default false)` — 启用后，目标任务完成时还原切换前的 BGM
      - `targetTask (TaskSO)` — 触发还原的目标任务定义资产
    - `[Header("Stop 参数")]`
      - `stopFadeOut (bool, default true)`, `stopFadeOutDuration (float, default 0.5f)`
    - `[Header("Play 参数")]`
      - `newClip (AudioClip)`, `loop (bool, default true)`, `playFadeIn (bool, default true)`, `playFadeInDuration (float, default 1f)`, `volume (float, [Range(0,1)], default 1f)`
  - Implement `IInteractable` properties:
    - `public bool CanInteract { get => canInteract; set => canInteract = value; }`
    - `public int  Priority    { get => priority;    set => priority    = value; }`
  - Add private runtime state:
    - `private bool _triggered = false;`
    - `private AudioClip _previousClip;`
  - Implement `OnEnable` / `OnDisable` subscribing/unsubscribing `HandleTaskCompleted` to `TaskEventBus.OnTaskCompleted`
  - Implement `InteractWith(PlayerInteractor player)`:
    - Null guard: if `newClip == null` → `Debug.LogError(...)` with `this` context, return
    - Once guard: if `interactOnce && _triggered` → return
    - `_triggered = true`
    - `_previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM)`
    - `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)`
    - `AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)`
  - Implement `HandleTaskCompleted(TaskRuntimeData data)` (identical logic to `TaskBGMSwitcher`):
    - `if (!restoreOnTaskComplete) return;`
    - `if (!_triggered) return;`
    - `if (targetTask == null) return;`
    - `if (data.Definition.taskIndex != targetTask.taskIndex) return;`
    - `_triggered = false;`
    - `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);`
    - `if (_previousClip != null) AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f);`

## Unity Editor Setup

- [ ] **Verify script compiles** — open Unity Editor, confirm no compiler errors
- [ ] **Add component** to a scene GameObject that the player can look at and interact with
  - The GameObject must have a `Collider` (non-trigger) for `PlayerDetector` raycast detection
- [ ] **Wire Inspector references:**
  - Assign an `AudioClip` to `New Clip`
  - If using `restoreOnTaskComplete`: check the bool and assign a `TaskSO` to `Target Task`
  - Adjust Stop/Play parameters and `Interact Once` as desired

## Verification Checklist

- [ ] Component appears in `Add Component` menu (search "InteractableBGMSwitcher")
- [ ] All Inspector fields visible and editable
- [ ] Player looking at the object sees the interact prompt (requires `ISelectable` or interact prompt setup)
- [ ] Pressing interact switches the BGM with correct fade behaviour
- [ ] Null guard: `newClip` unassigned → LogError, no switch, no crash
- [ ] `interactOnce = true`: second interaction does NOT switch BGM again
- [ ] `interactOnce = false`: every interaction triggers the switch
- [ ] `restoreOnTaskComplete = false`: task completing has no effect on BGM
- [ ] `restoreOnTaskComplete = true`: completing `targetTask` restores `_previousClip` with fade
- [ ] If no BGM was playing before the interaction, `_previousClip` is null and only Stop is called on restore
- [ ] Non-matching task completing does NOT trigger restore
