# Tasks: Scene-Based BGM Switcher

## Implementation

- [x] **Create `SceneBGMSwitcher.cs`**
  - File: `Assets/Scripts/BGM/SceneBGMSwitcher.cs`
  - Add `using` directives: `PostMan.AudioSystem`, `UnityEngine`, `System.Collections.Generic`
  - Declare class in global namespace (no `namespace` block), inheriting `MonoBehaviour`
  - Add XML `/// <summary>` doc comment (Simplified Chinese)
  - Add Inspector fields with `[Header]` and `[Tooltip]` groups:
    - `[Header("场景触发配置")]` → `targetSceneAddress (string)`, `triggerOnce (bool, default true)`
    - `[Header("Stop 参数")]` → `stopFadeOut (bool, default true)`, `stopFadeOutDuration (float, default 0.5f)`
    - `[Header("Play 参数")]` → `newClip (AudioClip)`, `loop (bool, default true)`, `playFadeIn (bool, default true)`, `playFadeInDuration (float, default 1f)`, `volume (float, [Range(0,1)], default 1f)`
  - Declare private `bool _triggered = false`
  - Implement `OnEnable` / `OnDisable` to subscribe/unsubscribe `HandleSceneSwitchCompleted` to `GameSceneManager.OnSceneSwitchCompleted`
  - Implement `HandleSceneSwitchCompleted(IReadOnlyList<string> loadedScenes)`:
    - Null guard: if `newClip == null`, log error with `this` context and return
    - Address guard: if `targetSceneAddress` is null or empty, log error with `this` context and return
    - Once guard: if `triggerOnce && _triggered`, return
    - Scene check: if `!loadedScenes.Contains(targetSceneAddress)`, return
    - Set `_triggered = true`
    - Call `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)`
    - Call `AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)`

## Unity Editor Setup

- [ ] **Verify script compiles** — open Unity Editor, confirm no compiler errors in the Console
- [ ] **Create or select a persistent GameObject** in the scene (e.g., on the same object that triggers the scene switch, or a dedicated BGM controller object) and attach `SceneBGMSwitcher`
- [ ] **Wire Inspector references:**
  - Set `Target Scene Address` to the Addressable key of the target scene (same string used in `GameSceneManager.SwitchScenes`)
  - Assign an `AudioClip` to `New Clip`
  - Adjust Stop/Play parameters as desired
  - Set `Trigger Once` as appropriate (default `true` prevents re-triggering on repeated loads)

## Verification Checklist

- [ ] Component appears in `Add Component` menu (search "SceneBGMSwitcher")
- [ ] All Inspector fields are visible and editable
- [ ] Null guard logs a clear error if `newClip` is unassigned
- [ ] Address guard logs a clear error if `Target Scene Address` is empty
- [ ] Loading a scene with a non-matching address does NOT switch the BGM
- [ ] Loading the matching scene switches BGM with correct fade behaviour
- [ ] `triggerOnce = true`: reloading the same scene a second time does NOT re-trigger the switch
- [ ] `triggerOnce = false`: reloading the same scene re-triggers the switch
- [ ] Unsubscription in `OnDisable` prevents callbacks after the component is disabled/destroyed
