# Design: Fix Scene Unload Failure for Non-Addressable-Loaded Scenes

## Root Cause

`GameSceneManager._loadedScenes` (a `Dictionary<string, AsyncOperationHandle<SceneInstance>>`) is written only inside `LoadSceneAsync` (line 140 of `GameSceneManager.cs`). Scenes that are already active in the hierarchy when the manager initialises — e.g., the initial scene placed in Build Settings — have no entry in this dictionary. `UnloadSceneAsync` queries the dictionary first (line 152) and returns early with a warning if the key is missing, so those scenes are never unloaded.

## Fix

Extend `UnloadSceneAsync` with a fallback branch that invokes the standard `SceneManager.UnloadSceneAsync(sceneName)` when no Addressables handle is cached. The scene name is extracted from the address string (last path segment, extension stripped).

### Modified file

`Assets/Scripts/SceneManagement/GameSceneManager.cs` — only `UnloadSceneAsync` is changed.

### Detailed change

```
UnloadSceneAsync(string address)
  1. TryGetValue(address, handle) → if found, use Addressables path (existing)
  2. NEW: if not found, derive sceneName from address
     → sceneName = Path.GetFileNameWithoutExtension(address)
       (handles both bare keys like "MainScene" and path keys like
        "Assets/Scenes/MainScene.unity")
  3. NEW: check SceneManager.GetSceneByName(sceneName).isLoaded
     → if not loaded, log warning and return (scene truly not present)
  4. NEW: await SceneManager.UnloadSceneAsync(sceneName).AsTask()
     → on success: log success
     → on failure: log error
```

`System.IO.Path.GetFileNameWithoutExtension` handles both address forms without regex.

`AsyncOperation.AsTask()` is a helper that wraps `AsyncOperation` into `Task` via `TaskCompletionSource`. Since no such helper exists yet in the project, the fallback will await via a coroutine-free inline wrapper using `AsyncOperation` + `await` on a custom `IAwaiter`. The simplest production-safe approach for Unity async context is:

```csharp
// Inline wrapper — no new file needed, private static method
private static Task ToTask(AsyncOperation op)
{
    var tcs = new TaskCompletionSource<bool>();
    op.completed += _ => tcs.SetResult(true);
    return tcs.Task;
}
```

This is added as a private static helper inside `GameSceneManager`.

### No other files require modification.

## Data Flow After Fix

```
UnloadSceneAsync("MainScene")
  → _loadedScenes.TryGetValue → not found
  → sceneName = "MainScene"
  → SceneManager.GetSceneByName("MainScene").isLoaded == true
  → SceneManager.UnloadSceneAsync("MainScene") wrapped in ToTask()
  → await → scene unloaded
  → log success
```

## Edge Cases

| Case | Behaviour |
|---|---|
| Address is Addressables key (e.g. `"RoomA"`) and scene was loaded via `GameSceneManager` | Existing Addressables path — unchanged |
| Address is Addressables key and scene was loaded at startup | Fallback path — unloaded via `SceneManager` |
| Address is a full path (e.g. `"Assets/Scenes/Room.unity"`) | `GetFileNameWithoutExtension` extracts `"Room"` — fallback matches correctly |
| Scene is not loaded at all | Early-return with warning — no crash |
