# Tasks: Collider-Triggered BGM Switcher

## Implementation

- [x] **Create `ColliderBGMSwitcher.cs`**
  - File: `Assets/Scripts/BGM/ColliderBGMSwitcher.cs`
  - Add `using` directives: `PostMan.AudioSystem`, `UnityEngine`
  - Declare class in global namespace (no `namespace` block), inheriting `MonoBehaviour`
  - Add XML `/// <summary>` doc comment (Simplified Chinese)
  - Add Inspector fields with `[Header]` and `[Tooltip]` groups:
    - `[Header("触发配置")]` → `triggerTag (string, default "Player")`, `triggerOnce (bool, default true)`
    - `[Header("Stop 参数")]` → `stopFadeOut (bool, default true)`, `stopFadeOutDuration (float, default 0.5f)`
    - `[Header("Play 参数")]` → `newClip (AudioClip)`, `loop (bool, default true)`, `playFadeIn (bool, default true)`, `playFadeInDuration (float, default 1f)`, `volume (float, [Range(0,1)], default 1f)`
  - Declare private `bool _triggered = false`
  - Implement `OnTriggerEnter(Collider other)`:
    - Null guard: if `newClip == null`, log error with `this` context and return
    - Tag filter: if `triggerTag` is non-empty and `other.CompareTag(triggerTag)` is false, return
    - Once guard: if `triggerOnce && _triggered`, return
    - Set `_triggered = true`
    - Call `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)`
    - Call `AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)`

## Unity Editor Setup

- [ ] **Verify script compiles** — open Unity Editor, confirm no compiler errors in the Console
- [ ] **Create a zone GameObject** in the scene:
  - Add a `Collider` component (e.g., Box Collider) and check **Is Trigger**
  - Attach `ColliderBGMSwitcher`
- [ ] **Wire Inspector references:**
  - Set `Trigger Tag` to match the player's tag (default `"Player"`)
  - Assign an `AudioClip` to `New Clip`
  - Adjust Stop/Play parameters as desired
- [ ] **Confirm the player has a Rigidbody** (required for `OnTriggerEnter` to fire)

## Verification Checklist

- [ ] Component appears in `Add Component` menu (search "ColliderBGMSwitcher")
- [ ] All Inspector fields are visible and editable
- [ ] Null guard logs a clear error if `newClip` is unassigned
- [ ] A non-matching tag object entering the collider does NOT switch the BGM
- [ ] Player entering the collider switches BGM with correct fade behaviour
- [ ] `triggerOnce = true`: re-entering the zone does NOT trigger a second switch
- [ ] `triggerOnce = false`: re-entering the zone triggers the switch again
- [ ] `triggerTag` set to empty string: any collider entry triggers the switch
