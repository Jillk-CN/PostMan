# Proposal: SceneBGMSwitcher sceneOrder Branch

## Problem

`SceneBGMSwitcher` currently plays a single BGM clip whenever a specific scene address is loaded. Some scenes require a different BGM depending on *which visit* to that scene it is — for example, the first time the player loads a particular scene versus a later revisit. There is no way to configure this branching without writing new code.

## Solution

Add a second trigger path to `SceneBGMSwitcher`: an optional `sceneOrder`-based branch. The component registers itself with `SceneInitializer` via `Register(SceneInitHandler)` and, when the callback fires, compares the received `sceneOrder` against a configurable `targetSceneOrder`. If the value matches, it plays a separately-configured `newClip2` instead of going through the address-match path. The existing address-match path is preserved unchanged.

The `sceneOrder` mechanism is already used by all `Init*` components (`InitalizeObjects`, `InitializeInteractables`, etc.) and follows the same pattern: register in `OnEnable`, check `targetSceneOrder != sceneOrder` early return in the callback.

## Key Decisions

- **Additive, not replacing** — the address-based trigger in `SceneBGMSwitcher` stays intact. The `sceneOrder` branch is an independent second trigger on the same component, executed from a separate callback.
- **Register in OnEnable / unregister not needed** — `SceneInitializer.initOperation` is cleared to `null` after every fire, so re-registration happens automatically each `OnEnable` (matching the existing Init pattern). No explicit unsubscribe is required.
- **Separate clip (`newClip2`)** — reuses the Stop/Play parameter group (stop fade, play fade, loop, volume) but plays `newClip2`. Keeps the Inspector footprint small.
- **`triggerOnce2` guard** — an independent once-guard for the `sceneOrder` branch, matching the pattern of the existing `triggerOnce` flag.
- **`PostMan.Scene` using** — `SceneBGMSwitcher` already imports `PostMan.Scene`; no new dependencies needed.

## Non-goals

- Does not modify `SceneInitializer` or any other Init component.
- Does not support multiple `sceneOrder` values on a single component; use additional components for multiple orders.
- Does not queue or crossfade more than two tracks simultaneously.
- Does not add new `AudioManager` or `GameSceneManager` methods.
