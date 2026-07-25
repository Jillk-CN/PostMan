# Tasks: Scene BGM Restore on Unload

## Implementation

- [x] **Modify `SceneBGMSwitcher.cs`**
  - File: `Assets/Scripts/BGM/SceneBGMSwitcher.cs`
  - Add `restoreOnUnload` field inside `[Header("场景触发配置")]`, after `triggerOnce`:
    - `/// <summary>` — 启用后，当目标场景被卸载时自动还原切换前的 BGM
    - `[Tooltip]` — 勾选后，目标场景卸载时停止当前 BGM 并还原加载前的曲目；未勾选则不做任何处理
    - `[SerializeField] private bool restoreOnUnload = false;`
  - Add `_previousClip` field to the runtime state section:
    - `/// <summary>` — 加载 targetSceneAddress 前正在播放的 BGM 快照，卸载时用于还原
    - `private AudioClip _previousClip;`
  - In `HandleSceneSwitchCompleted`, **before** the `triggerOnce && _triggered` guard (step 4), insert the unload detection branch:
    ```csharp
    // 卸载还原分支：已切换过 BGM 且本次切换不含目标场景 → 目标场景已被卸载
    if (restoreOnUnload && _triggered && !loadedScenes.Contains(targetSceneAddress))
    {
        _triggered = false;
        AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration);
        if (_previousClip != null)
            AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, 1f);
        return;
    }
    ```
  - In `HandleSceneSwitchCompleted`, **before** `_triggered = true` (in the load path), insert the snapshot:
    ```csharp
    // 快照当前 BGM，供卸载时还原
    _previousClip = AudioManager.Instance.GetClip(AudioTrackId.BGM);
    ```

## Unity Editor Setup

- [ ] **Verify script compiles** — open Unity Editor, confirm no compiler errors
- [ ] **Select the GameObject** with `SceneBGMSwitcher` attached
- [ ] **Enable restore:** check `Restore On Unload`
- [ ] **Verify existing fields** (`Target Scene Address`, `New Clip`, Stop/Play params) are still intact after the change

## Verification Checklist

- [ ] `restoreOnUnload = false` (default): scene unload fires `HandleSceneSwitchCompleted` but no restore occurs — existing behaviour unchanged
- [ ] `restoreOnUnload = true`: loading the target scene snapshots the previous BGM and switches to `newClip`
- [ ] `restoreOnUnload = true`: unloading the target scene (switching to another scene that doesn't include it) stops `newClip` and restores `_previousClip`
- [ ] If no BGM was playing before the load, `_previousClip` is null and only `Stop` is called on unload
- [ ] After restore, `_triggered` is reset to `false`; reloading the scene again triggers the switch a second time
- [ ] `triggerOnce = true` with `restoreOnUnload = true`: load→unload→reload cycle works (restore resets `_triggered`, allowing the second load to fire)
- [ ] The `sceneOrder` branch (`newClip2`) is not affected by this change
