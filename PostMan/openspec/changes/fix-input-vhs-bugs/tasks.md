# Tasks: Fix InputSource Reference Loss and VHS Filter Reset on Scene Switch

## Bug 1 — InputSource Re-acquisition

### Task 1.1 — Add `RefreshInputActions()` to `IInputSource`

- [x] Open `Assets/Scripts/InputManagement/IInputSource.cs`
- [x] Add the method signature to the interface:
  ```csharp
  /// <summary>在场景切换后重新从 GameInputManager 获取 PlayerInputActions 引用。</summary>
  void RefreshInputActions();
  ```

### Task 1.2 — Implement `RefreshInputActions()` in `PlayerMotionInputSource`

- [x] Open `Assets/Scripts/InputManagement/PlayerMotionInputSource.cs`
- [x] Add the method:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
  }
  ```

### Task 1.3 — Implement `RefreshInputActions()` in `PlayerSightInputSource`

- [x] Open `Assets/Scripts/InputManagement/PlayerSightInputSource.cs`
- [x] Add the method:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
  }
  ```

### Task 1.4 — Implement `RefreshInputActions()` in `PlayerInteractInputSource`

- [x] Open `Assets/Scripts/InputManagement/PlayerInteractInputSource.cs`
- [x] Add the method:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
  }
  ```
- [x] Note: the `performed` callback is registered in `Awake()` on the old `inputActions`. After `RefreshInputActions()`, re-register it on the new instance. Update `RefreshInputActions()` to also re-subscribe the callback:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
      // 重新注册 performed 回调到新的 inputActions 实例
      this.inputActions.Player.Interact.performed += (context) =>
      {
          this.interacting = true;
      };
  }
  ```

### Task 1.5 — Implement `RefreshInputActions()` in `PauseInputSource`

- [x] Open `Assets/Scripts/InputManagement/PauseInputSource.cs`
- [x] Add the method:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
  }
  ```

### Task 1.6 — Implement `RefreshInputActions()` in `UIReturnInputSource`

- [x] Open `Assets/Scripts/InputManagement/UIReturnInputSource.cs`
- [x] Add the method:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
  }
  ```

### Task 1.7 — Implement `RefreshInputActions()` in `VHSToggleInputSource`

- [x] Open `Assets/Scripts/InputManagement/VHSToggleInputSource.cs`
- [x] Add the method:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
  }
  ```

### Task 1.8 — Implement `RefreshInputActions()` in `DialogueInputSource`

- [x] Open `Assets/Scripts/InputManagement/DialogueInputSource.cs`
- [x] Add the method:
  ```csharp
  public void RefreshInputActions()
  {
      this.inputActions = GameInputManager.Instance.GetInputAction();
  }
  ```

### Task 1.9 — Call `RefreshInputActions()` from `GameInputManager.Init()`

- [x] Open `Assets/Scripts/InputManagement/GameInputManager.cs`
- [x] After the line `sources = this.GetComponents<IInputSource>().ToList();`, add:
  ```csharp
  // 场景切换后重新绑定：所有 InputSource 重新获取当前 inputActions 引用
  foreach (var src in sources)
  {
      src.RefreshInputActions();
  }
  ```
- [x] Verify the complete `Init()` order: create `inputActions` → get `sources` → refresh all sources → `HideCursor()`

---

## Bug 2 — VHS Filter Settings Persistence

### Task 2.1 — Add static persistence field to `VHSFilterController`

- [x] Open `Assets/Scripts/VHSFilter/VHSFilterController.cs`
- [x] Below the `// 运行时状态` comment block, add a static field above the instance field:
  ```csharp
  // 跨场景持久化：静态字段在场景重新加载后仍保持上次的设置
  private static VHSSettings s_persistedSettings = VHSSettings.Default;
  ```
- [x] Change the instance field initializer from:
  ```csharp
  private VHSSettings _settings = VHSSettings.Default;
  ```
  to:
  ```csharp
  private VHSSettings _settings = s_persistedSettings;
  ```

### Task 2.2 — Persist settings on every `Push()`

- [x] In the same file, locate the `Push()` method
- [x] After `_settings = newSettings;`, add:
  ```csharp
  s_persistedSettings = newSettings;   // 持久化到静态字段，跨场景保留
  ```
- [x] Final `Push()` should look like:
  ```csharp
  private void Push(VHSSettings newSettings)
  {
      _settings = newSettings;
      s_persistedSettings = newSettings;
      _effect?.Apply(_settings);
      _volumeCtrl?.Apply(_settings);
      SyncUI();
  }
  ```

### Task 2.3 — Verify `Start()` re-applies persisted settings correctly

- [x] Confirm `Start()` calls `Push(_settings)` — since `_settings` is now initialised from `s_persistedSettings`, this will push the last known settings to `VHSFilterEffect` and `VHSVolumeController` on scene load. No change needed if `Start()` already reads `_settings` (it does).

### Task 2.4 — Manual verification in Unity Editor

- [ ] Enter Play mode in the game scene
- [ ] Press O to open the VHS panel
- [ ] Enable Noise and set strength to a visible value (e.g. 0.5)
- [ ] Close the VHS panel
- [ ] Reopen the VHS panel — confirm Noise is still enabled with 0.5 strength
- [ ] Trigger a scene switch (or reload the scene) — confirm VHS effect is still applied and panel shows correct values on reopen
