# Tasks: TaskBGMSwitcher Restore on Task Complete

## Implementation

- [x] **Modify `TaskBGMSwitcher.cs`**
  - File: `Assets/Scripts/BGM/TaskBGMSwitcher.cs`
  - Add `restoreOnTaskComplete` field inside `[Header("Task 触发配置")]`, after `targetTask`:
    - `/// <summary>` — 启用后，当目标任务完成时自动还原切换前的 BGM
    - `[Tooltip]` — 勾选后，目标任务完成时停止当前 BGM 并还原任务启动前的曲目；未勾选则不做任何处理
    - `[SerializeField] private bool restoreOnTaskComplete = false;`
  - Add runtime state fields (before the lifecycle section):
    - `/// <summary>` — BGM 切换保护标志；HandleTaskStarted 成功执行后置为 true
    - `private bool _triggered = false;`
    - `/// <summary>` — 任务启动前正在播放的 BGM 快照，任务完成时用于还原
    - `private AudioClip _previousClip;`
  - Update `OnEnable` to also subscribe `HandleTaskCompleted` to `TaskEventBus.OnTaskCompleted`
  - Update `OnDisable` to also unsubscribe `HandleTaskCompleted` from `TaskEventBus.OnTaskCompleted`
  - In `HandleTaskStarted`, immediately before `AudioManager.Instance.Stop(...)`:
    ```csharp
    _triggered    = true;
    _previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM);
    ```
  - Add private method `HandleTaskCompleted(TaskRuntimeData data)`:
    - `if (!restoreOnTaskComplete) return;`
    - `if (!_triggered) return;`
    - `if (data.Definition.taskIndex != targetTask.taskIndex) return;`
    - `_triggered = false;`
    - `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);`
    - `if (_previousClip != null) AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f);`

## Unity Editor Setup

- [ ] **Verify script compiles** — open Unity Editor, confirm no compiler errors
- [ ] **Select the GameObject** with `TaskBGMSwitcher` attached
- [ ] **Enable restore:** check `Restore On Task Complete`
- [ ] **Verify existing fields** (`targetTask`, `newClip`, Stop/Play params) are still intact

## Verification Checklist

- [ ] `restoreOnTaskComplete = false` (default): task completing has no effect on BGM — existing behaviour unchanged
- [ ] `restoreOnTaskComplete = true`: starting the target task snapshots the previous BGM and switches to `newClip`
- [ ] `restoreOnTaskComplete = true`: completing the target task stops `newClip` and restores `_previousClip`
- [ ] If no BGM was playing before the task started, `_previousClip` is null and only `Stop` is called on complete
- [ ] Non-matching task completing does NOT trigger the restore
- [ ] `_triggered` resets to `false` after restore; the component is idempotent if `OnTaskCompleted` fires again
- [ ] `targetTask` null guard in `HandleTaskCompleted` — if `targetTask` is null, no crash (the existing null check in `HandleTaskStarted` would have prevented `_triggered` from being set, so restore branch exits early at `!_triggered`)
