# Tasks: Fix InputSource State Corruption on Scene Reload

## Task 1 — Add `ResetToGameDefaults()` to `GameInputManager`

**File:** `Assets/Scripts/InputManagement/GameInputManager.cs`

- [x] Open `GameInputManager.cs`.
- [x] After the `DisablePlayerAllInput()` method (line ~100), add the following public method:

```csharp
/// <summary>
/// 将所有输入源重置为游戏场景默认状态。
/// 在每次加载游戏场景后调用，防止上一次游戏会话中的启用/禁用状态污染新会话。
/// </summary>
public void ResetToGameDefaults()
{
    // 玩家核心输入：默认全部启用
    SetInputSystemSource<PlayerMotionInputSource>(true);
    SetInputSystemSource<PlayerSightInputSource>(true);
    SetInputSystemSource<PlayerInteractInputSource>(true);

    // 暂停键：游戏开始即可用
    SetInputSystemSource<PauseInputSource>(true);

    // 按需输入源：由各自系统在需要时启用，此处确保初始为关闭
    SetInputSystemSource<UIReturnInputSource>(false);
    SetInputSystemSource<VHSToggleInputSource>(false);
    SetInputSystemSource<DialogueInputSource>(false);
}
```

- [x] Verify the file compiles without errors in Unity (no new `using` directives needed — all referenced types are already in scope).

---

## Task 2 — Call `ResetToGameDefaults()` from `GameSceneManager`

**File:** `Assets/Scripts/SceneManagement/GameSceneManager.cs`

- [x] Open `GameSceneManager.cs`.
- [x] Locate the comment `// ── 阶段 1.5：传送玩家到出生点 ──` (around line 97–99).
- [x] Insert the reset call *before* the teleport block, and update the section comment:

```csharp
// ── 阶段 1.5：重置输入源 + 传送玩家到出生点 ──
// 所有目标场景已加载完毕，在卸载旧场景前将输入源归位，防止旧会话状态污染
GameInputManager.Instance?.ResetToGameDefaults();

if (spawnPoint.HasValue)
    TeleportPlayer(spawnPoint.Value);
```

- [x] Verify no new `using` directives are needed (`PostMan.InputManagement` should already be imported or the call resolves via the namespace in scope — check the existing `using` list at the top of the file and add `using PostMan.InputManagement;` if absent).
- [x] Confirm the file compiles without errors.

---

## Task 3 — Manual Verification in Unity Editor

- [ ] In Unity Editor, open **Edit → Clear All PlayerPrefs** to start from a clean state.
- [ ] Enter **Play mode** starting from the Title scene.
- [ ] Click "Start Game" (or whichever button triggers `SwitchScenes` to the game scene).
  - Verify: player can move (WASD), look (mouse), interact (E), and pause (Esc).
- [ ] Press Esc to open the pause menu, then select "Return to Title" (or equivalent).
  - Verify: Title scene loads correctly.
- [ ] From Title, click "Start Game" again to re-enter the game scene.
  - Verify: player can move, look, interact, and pause — no frozen inputs.
- [ ] Repeat the Title → Game cycle a third time to confirm the fix is stable.
- [ ] Open a UI panel mid-game (e.g., ViewImagePanel or TextPanel) so that `DisablePlayerAllInput()` is triggered, then exit to Title, then re-enter.
  - Verify: inputs work correctly in the new session despite having been disabled before exit.
