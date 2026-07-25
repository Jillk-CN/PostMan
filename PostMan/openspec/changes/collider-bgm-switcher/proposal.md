# Proposal: Collider-Triggered BGM Switcher

## Problem

`TaskBGMSwitcher` handles BGM switching via narrative task events, but some BGM transitions need to be purely spatial — "when the player walks into this zone, switch the music." There is currently no way to trigger a BGM change by entering a physics trigger volume without writing code.

## Solution

Create a new `MonoBehaviour` — `ColliderBGMSwitcher` — placed inside `Assets/Scripts/BGM/`. It uses `OnTriggerEnter` to detect when an object (typically the player) enters a configured trigger collider, then calls `AudioManager.Instance.Stop(BGM, ...)` followed by `AudioManager.Instance.Play(BGM, ...)` using the same API and the same Inspector field layout as `TaskBGMSwitcher`. The triggering object can be filtered by tag to prevent unintended activations.

## Key Decisions

- **BGM folder** — same location as `BGMPlayer.cs` and `TaskBGMSwitcher.cs`; designers find all BGM-related components together.
- **`OnTriggerEnter` only** — zone entry is the most natural spatial trigger; exit/stay handling is out of scope.
- **Tag filter** — an optional `triggerTag` field (default `"Player"`) guards against non-player colliders activating the switch.
- **triggerOnce guard** — a `bool _triggered` flag prevents the switch from firing repeatedly if the player re-enters the zone.
- **Play/Stop parameter parity with `TaskBGMSwitcher`** — identical `[Header]` groups and field names so designers have a consistent Inspector experience across both switcher types.

## Non-goals

- Does not handle `OnTriggerExit` or `OnTriggerStay`.
- Does not implement crossfade between more than two tracks.
- Does not add any new `AudioManager` methods or modify the existing audio pipeline.
- Does not create any UI or visual feedback.
- Does not support multiple collider zones on a single component.
