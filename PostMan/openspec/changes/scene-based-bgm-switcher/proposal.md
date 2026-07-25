# Proposal: Scene-Based BGM Switcher

## Problem

`BGMPlayer` handles initial playback on Awake, `TaskBGMSwitcher` switches BGM in response to narrative task events, and `ColliderBGMSwitcher` switches BGM when the player enters a trigger zone. However, none of these handle the common case where the BGM should change when the game loads a specific scene. There is currently no way for designers to say "whenever scene X finishes loading, switch to this BGM" without writing code.

## Solution

Create a new `MonoBehaviour` — `SceneBGMSwitcher` — placed inside `Assets/Scripts/BGM/`. It subscribes to `GameSceneManager.OnSceneSwitchCompleted`, checks whether the completed scene switch included a configured target scene address, then calls `AudioManager.Instance.Stop(BGM, ...)` followed by `AudioManager.Instance.Play(BGM, ...)` using the same API and Inspector field layout as `TaskBGMSwitcher` and `ColliderBGMSwitcher`. A `triggerOnce` guard prevents the switch from repeating if the same scene is reloaded unexpectedly.

## Key Decisions

- **BGM folder** — consistent with `BGMPlayer.cs`, `TaskBGMSwitcher.cs`, and `ColliderBGMSwitcher.cs`; all BGM-related components live together.
- **`OnSceneSwitchCompleted` only** — fires after both load and unload phases complete, ensuring the scene is fully ready before the BGM transitions. No partial-load edge cases.
- **Scene address match** — the designer supplies the Addressable key string (same key used in `GameSceneManager.SwitchScenes`). A `Contains` check on the loaded-scene list allows the switcher to react to multi-scene loads correctly.
- **`triggerOnce` guard** — prevents repeated BGM transitions if the scene is loaded more than once in a session.
- **Play/Stop parameter parity** — identical `[Header]` groups and field names as `TaskBGMSwitcher` and `ColliderBGMSwitcher` for a consistent Inspector experience.

## Non-goals

- Does not handle partial scene loads (e.g., only some scenes in a batch load successfully).
- Does not implement a queue or crossfade between more than two tracks.
- Does not add any new `AudioManager` or `GameSceneManager` methods.
- Does not create any UI or visual feedback for the transition.
- Does not support multiple target scene addresses on a single component.
