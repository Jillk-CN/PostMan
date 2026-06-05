## Proposal: Fix Scene Unload Failure for Non-Addressable-Loaded Scenes

### What

`GameSceneManager.UnloadSceneAsync` silently skips unloading any scene whose address is absent from the `_loadedScenes` handle cache. Scenes that were present in the Unity build at startup (i.e., not loaded via `GameSceneManager.LoadSceneAsync`) are never entered into that cache, so they are never unloaded when passed to `SwitchScenes`.

### Why

During testing, scenes listed in `scenesToUnload` that were loaded through the Build Settings (not through `GameSceneManager`) were not unloaded after calling `SwitchScenes`. The root cause is that `_loadedScenes` is populated exclusively in `LoadSceneAsync` (line 140). Any scene already present in the hierarchy at startup has no cached handle, causing `UnloadSceneAsync` (line 152–156) to hit the early-return guard and log only a warning.

### Approach

Add a fallback path in `UnloadSceneAsync`: when no handle is found in `_loadedScenes`, fall back to `SceneManager.UnloadSceneAsync(sceneName)` using the scene name derived from the Addressable address string. This handles initial Build Settings scenes without requiring any external registration step or architectural change.

The address string is treated as both the Addressable key and the scene name for the fallback lookup (Unity's `SceneManager` matches on scene name, which equals the file name without extension — the same value typically used as the Addressable key).

### Non-goals

- Registering initial scenes into `_loadedScenes` at startup (adds complexity, fragile if address ≠ scene name)
- Migrating all scenes to Addressables-only loading (larger architectural change, out of scope)
- Changing the load path — only the unload fallback is modified
