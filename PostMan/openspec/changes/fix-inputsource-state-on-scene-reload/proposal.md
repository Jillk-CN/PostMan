# Proposal: Fix InputSource State Corruption on Scene Reload

## Problem

When the player exits from the game scene back to the Title scene and then re-enters the game scene, the input sources managed by `GameInputManager` enter an incorrect enable/disable state. Symptoms include player movement, camera look, and interaction inputs not responding, or the pause key misbehaving.

### Root Cause

`GameInputManager` uses `MonoSingleton<T>` with `DontDestroyOnLoad`, so the manager object persists across scene loads. Its `sources` list is populated once in `Init()` via `GetComponents<IInputSource>()` — this captures the `IInputSource` components that live on the same `DontDestroyOnLoad` GameObject, so those references stay valid.

The real issue is **stale enabled/disabled state carried over between sessions**. When the game scene is loaded the first time, certain systems (e.g., `PausePanel`, `ViewImagePanel`, `TextPanel`) call `DisablePlayerAllInput()` to suppress movement/look while a UI panel is open. If the player exits to Title *while such a panel is still active*, those sources remain disabled. When the game scene is reloaded, the sources are never reset to their default enabled state because nothing re-initialises `GameInputManager` (it's already alive and `Init()` won't run again).

A secondary concern: code that calls `GameInputManager.Instance` from `Awake()` in scene objects (e.g., `PlayerMotionInputSource`) may execute before `MonoSingleton<T>.Instance` has been set, causing `NullReferenceException` if the scene load order changes.

## Proposed Solution

Add a `ResetToGameDefaults()` method to `GameInputManager` that restores every input source to its correct "fresh game session" state (player inputs enabled, UI inputs disabled). Call this method from `GameSceneManager.SwitchScenes()` immediately after the new game scene finishes loading and before the old scenes are unloaded.

This is a minimal, targeted fix: no new files, no architectural changes — just a reset hook at the scene boundary.

## Non-goals

- Refactoring `MonoSingleton<T>` or the scene loading pipeline.
- Fixing unrelated input issues in menus or cutscenes.
- Adding cursor-state reset (already handled separately by scene entrypoints).
- Supporting partial or additive scene reloads beyond the existing `SwitchScenes` API.
