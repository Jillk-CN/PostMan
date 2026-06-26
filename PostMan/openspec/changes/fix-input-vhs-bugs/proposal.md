# Proposal: Fix InputSource Reference Loss and VHS Filter Reset on Scene Switch

## Problem

Two unrelated bugs both manifest after a scene transition.

### Bug 1 — InputSource references become stale after scene switch

Every `IInputSource` component (`PlayerMotionInputSource`, `PlayerSightInputSource`, `PlayerInteractInputSource`, `PauseInputSource`, `UIReturnInputSource`, `VHSToggleInputSource`, `DialogueInputSource`) caches a `PlayerInputActions` reference in `Awake()` via `GameInputManager.Instance.GetInputAction()`.

`GameInputManager` does **not** use `DontDestroyOnLoad` (the call is commented out). When a scene is unloaded, the `GameInputManager` object is destroyed, and its `PlayerInputActions` instance is disposed. The new scene creates a fresh `GameInputManager` with a new `PlayerInputActions`, but the components on the new scene's objects still need to re-acquire that reference. After `SwitchScenes` completes, any InputSource whose `Awake` ran before the new `GameInputManager` was ready, or that holds a reference to the old destroyed instance, will fail silently or throw `NullReferenceException`.

### Bug 2 — VHS filter panel resets all settings on open

`VHSFilterController` is a plain `MonoBehaviour` in the scene. Its `_settings` field is initialised to `VHSSettings.Default` at field-declaration time and `Start()` immediately calls `Push(_settings)`, which pushes default values to `VHSFilterEffect` and `VHSVolumeController`. Because the controller lives only in the scene (not `DontDestroyOnLoad`), every scene reload resets it. Furthermore, `VHSFilterEffect` is a `ScriptableRendererFeature` on a persistent URP Renderer Asset — it stores `_currentSettings`, but this state is also reset during `Create()` which is called each time the renderer is initialised.

The result: modifications made in the VHS panel during a session are lost when the scene changes or the panel is reopened (since `SyncBeforeShow` syncs UI to `_settings`, which is always `Default`).

## Proposed Solution

**Bug 1:** Add a `RefreshInputActions()` method to each `IInputSource` that re-acquires the `PlayerInputActions` reference from the current `GameInputManager`. Call `RefreshInputActions()` on all sources from `GameInputManager.Init()` (which runs when the new scene's manager is found) or alternatively from `GameSceneManager` after scene load — as specified: re-find the `GameInputManager`'s InputSources in the scene after each scene switch.

**Bug 2:** Persist `VHSSettings` across scene transitions by storing it in a static field on `VHSFilterController`, so re-initialised controller instances restore the last applied settings instead of defaulting.

## Non-goals

- Refactoring `MonoSingleton<T>` or making `GameInputManager` `DontDestroyOnLoad`.
- Persisting VHS settings to disk / PlayerPrefs.
- Fixing the `UIReturnInputSource.Disable()` bug (calls `Enable()` instead of `Disable()`) — separate concern.
- Adding any new UI for VHS settings management.
