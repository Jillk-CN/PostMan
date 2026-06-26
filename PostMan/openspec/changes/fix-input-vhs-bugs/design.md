# Design: Fix InputSource Reference Loss and VHS Filter Reset on Scene Switch

## Overview

Two independent bugs, two targeted fixes. No new files, no architectural changes.

---

## Fix 1 — InputSource Re-acquisition After Scene Switch

### Root Cause

Every InputSource caches `PlayerInputActions` in `Awake()`:

```csharp
// In PlayerMotionInputSource (and all other sources):
private void Awake()
{
    this.inputActions = GameInputManager.Instance.GetInputAction();
}
```

`GameInputManager` does **not** call `DontDestroyOnLoad` (the line is commented out in `GameInputManager.cs`). Each scene owns its own `GameInputManager` instance. When a scene is unloaded, its `GameInputManager` is destroyed and `inputActions.Dispose()` is called in `OnDestroy()`. The new scene's `GameInputManager.Init()` creates a fresh `PlayerInputActions`.

Components that Awake'd before the new `GameInputManager` existed, or that somehow hold the old reference, end up pointing at a disposed `PlayerInputActions`. The fix requested: after a scene switch, every InputSource must re-acquire the `inputActions` reference by finding the current scene's `GameInputManager`.

### Solution

Add a `RefreshInputActions()` method to `IInputSource` — or more practically, call a refresh from `GameInputManager.Init()` since `Init()` runs when the new `GameInputManager` singleton is first accessed (via `MonoSingleton<T>.Instance`). Because all InputSources live on the **same GameObject** as `GameInputManager`, their `Awake()` runs in the same scene start phase. However, to guarantee correctness across all orderings, `GameInputManager.Init()` should call `RefreshInputActions()` on each source after it builds the `sources` list.

**Change 1a — Add `RefreshInputActions()` to `IInputSource`:**

File: `Assets/Scripts/InputManagement/IInputSource.cs`

```csharp
public interface IInputSource
{
    bool Enabled { get; }
    void Enable();
    void Disable();
    /// <summary>在场景切换后重新从 GameInputManager 获取 PlayerInputActions 引用。</summary>
    void RefreshInputActions();
}
```

**Change 1b — Implement `RefreshInputActions()` in each InputSource:**

Each source already has `private PlayerInputActions inputActions`. Add:

```csharp
public void RefreshInputActions()
{
    this.inputActions = GameInputManager.Instance.GetInputAction();
}
```

This applies to all 7 sources:
- `Assets/Scripts/InputManagement/PlayerMotionInputSource.cs`
- `Assets/Scripts/InputManagement/PlayerSightInputSource.cs`
- `Assets/Scripts/InputManagement/PlayerInteractInputSource.cs`
- `Assets/Scripts/InputManagement/PauseInputSource.cs`
- `Assets/Scripts/InputManagement/UIReturnInputSource.cs`
- `Assets/Scripts/InputManagement/VHSToggleInputSource.cs`
- `Assets/Scripts/InputManagement/DialogueInputSource.cs`

**Change 1c — Call `RefreshInputActions()` from `GameInputManager.Init()`:**

File: `Assets/Scripts/InputManagement/GameInputManager.cs`

After rebuilding the `sources` list, call `RefreshInputActions()` on each source so they all point to the newly created `PlayerInputActions`:

```csharp
protected override void Init()
{
    base.Init();
    if (Instance != this)
    {
        Destroy(this.gameObject);
        return;
    }
    inputActions = new PlayerInputActions();
    sources = this.GetComponents<IInputSource>().ToList();

    // 场景切换后重新绑定：所有 InputSource 重新获取当前 inputActions 引用
    foreach (var src in sources)
    {
        src.RefreshInputActions();
    }

    HideCursor();
}
```

This means: whenever a new `GameInputManager` is initialised (i.e., a new scene starts), all co-located InputSources immediately re-acquire the fresh `PlayerInputActions` reference. No external caller needed.

---

## Fix 2 — VHS Filter Settings Persistence Across Scene Transitions

### Root Cause

`VHSFilterController` is a scene `MonoBehaviour`. Its state:

```csharp
private VHSSettings _settings = VHSSettings.Default;
```

is re-initialised every time the scene loads. `Start()` calls `Push(_settings)`, which pushes `VHSSettings.Default` to both `VHSFilterEffect` and `VHSVolumeController`, overwriting any previously applied settings. When the VHS panel is opened, `SyncBeforeShow()` syncs UI to `_settings` — which is always `Default` because the controller was just constructed from scratch.

The `VHSFilterEffect` (a `ScriptableRendererFeature` on the persistent URP Renderer Asset) stores `_currentSettings`, but its `Create()` method also resets it to `VHSSettings.Default` on each renderer initialisation.

### Solution

Store the last applied `VHSSettings` in a **static field** on `VHSFilterController`. On scene reload, the new controller instance reads from the static field instead of `VHSSettings.Default`, restoring the previous state before calling `Push`.

**Change 2 — Add static persistence to `VHSFilterController`:**

File: `Assets/Scripts/VHSFilter/VHSFilterController.cs`

```csharp
// 跨场景持久化：静态字段在场景重新加载后仍保持上次的设置
private static VHSSettings s_persistedSettings = VHSSettings.Default;

// 将实例字段初始化改为从静态字段读取
private VHSSettings _settings = s_persistedSettings;
```

In `Push()`, update the static field whenever settings change:

```csharp
private void Push(VHSSettings newSettings)
{
    _settings = newSettings;
    s_persistedSettings = newSettings;   // ← 持久化到静态字段
    _effect?.Apply(_settings);
    _volumeCtrl?.Apply(_settings);
    SyncUI();
}
```

In `Start()`, use the persisted settings (not `Default`) to re-apply the last state to the effect:

```csharp
private void Start()
{
    // 用持久化的设置（而非 Default）推送，确保效果与上次一致
    Push(_settings);
}
```

Because `_settings` is now initialised from `s_persistedSettings` at construction time, the first `Push` in `Start()` replays the last known settings to both `VHSFilterEffect` and `VHSVolumeController`.

---

## Affected Files Summary

| File | Change |
|------|--------|
| `Assets/Scripts/InputManagement/IInputSource.cs` | Add `RefreshInputActions()` to interface |
| `Assets/Scripts/InputManagement/PlayerMotionInputSource.cs` | Implement `RefreshInputActions()` |
| `Assets/Scripts/InputManagement/PlayerSightInputSource.cs` | Implement `RefreshInputActions()` |
| `Assets/Scripts/InputManagement/PlayerInteractInputSource.cs` | Implement `RefreshInputActions()` |
| `Assets/Scripts/InputManagement/PauseInputSource.cs` | Implement `RefreshInputActions()` |
| `Assets/Scripts/InputManagement/UIReturnInputSource.cs` | Implement `RefreshInputActions()` |
| `Assets/Scripts/InputManagement/VHSToggleInputSource.cs` | Implement `RefreshInputActions()` |
| `Assets/Scripts/InputManagement/DialogueInputSource.cs` | Implement `RefreshInputActions()` |
| `Assets/Scripts/InputManagement/GameInputManager.cs` | Call `RefreshInputActions()` on all sources after Init |
| `Assets/Scripts/VHSFilter/VHSFilterController.cs` | Add `s_persistedSettings` static field; use in `Push()` and field initializer |
