# Tasks: Fix Scene Unload Failure for Non-Addressable-Loaded Scenes

## Implementation Tasks

- [ ] In `Assets/Scripts/SceneManagement/GameSceneManager.cs`, add `using System.IO;` to the using block (needed for `Path.GetFileNameWithoutExtension`)

- [ ] In `GameSceneManager.cs`, add a private static helper method `ToTask(AsyncOperation op)` that wraps an `AsyncOperation` into a `Task` via `TaskCompletionSource<bool>`:
  ```csharp
  private static Task ToTask(AsyncOperation op)
  {
      var tcs = new TaskCompletionSource<bool>();
      op.completed += _ => tcs.SetResult(true);
      return tcs.Task;
  }
  ```

- [ ] In `GameSceneManager.cs`, modify `UnloadSceneAsync(string address)` to add a fallback branch after the `TryGetValue` early-return block:
  - When handle is not found in `_loadedScenes`, derive `sceneName` via `Path.GetFileNameWithoutExtension(address)`
  - Check `SceneManager.GetSceneByName(sceneName).isLoaded`; if false, log warning and return
  - Otherwise, await `ToTask(SceneManager.UnloadSceneAsync(sceneName))`
  - Log success or error accordingly

## Verification

- [ ] Open a test scene in Unity that is loaded via Build Settings (not via `GameSceneManager`)
- [ ] Attach a `ColliderSceneChange` or `InteractableSceneChange` component with the initial scene's address in `_scenesToUnload`
- [ ] Enter Play Mode, trigger the scene change
- [ ] Confirm in the Console: no "未在缓存中找到句柄" warning; instead the fallback log message appears and the scene is unloaded
- [ ] Confirm scenes loaded via `GameSceneManager` (Addressables path) still unload correctly — existing behaviour unchanged
