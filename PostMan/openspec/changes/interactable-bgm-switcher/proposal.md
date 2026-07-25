# Proposal: Interactable BGM Switcher

## Problem

All existing BGM switchers (`TaskBGMSwitcher`, `ColliderBGMSwitcher`, `SceneBGMSwitcher`) trigger automatically based on system events. There is no way to tie a BGM change directly to a player interaction — for example, "when the player picks up this item or activates this object, switch the BGM." Designers cannot attach a BGM change to any interactable prop without writing custom code.

## Solution

Create a new `MonoBehaviour` — `InteractableBGMSwitcher` — placed inside `Assets/Scripts/BGM/`. It implements `IInteractable` so it can sit alongside other interactable components on any scene GameObject. When `InteractWith` is called (player presses interact while looking at the object), it executes the same Stop+Play BGM switch used by all other switchers.

The component also includes the full `restoreOnTaskComplete` feature from `TaskBGMSwitcher`: it subscribes to `TaskEventBus.OnTaskCompleted` and, when a configured target task completes, restores the previously-playing BGM — making the task completion the natural end of this BGM section.

## Key Decisions

- **`IInteractable` implementation** — fits directly into the existing `PlayerInteractor` system without any new input handling. Multiple `IInteractable` components on the same GameObject compose naturally via priority ordering.
- **BGM folder** — consistent with all other BGM switchers.
- **Global namespace** — matches `BGMPlayer`, `TaskBGMSwitcher`, `ColliderBGMSwitcher`, and all concrete interactable objects.
- **`restoreOnTaskComplete` identical to `TaskBGMSwitcher`** — same fields, same lifecycle (`OnEnable`/`OnDisable` subscribe to `OnTaskCompleted`), same logic. No new patterns introduced.
- **`interactOnce` guard** — prevents repeated BGM switches if the player interacts with the object multiple times, matching `triggerOnce` semantics in other switchers.
- **`CanInteract` / `Priority`** — serialized Inspector fields with property accessors, following the existing IInteractable implementation pattern.

## Non-goals

- Does not add visual highlight or selection logic (that is handled by `ISelectable` and `PlayerDetector`).
- Does not add a separate restore path for when the interaction is "undone."
- Does not implement `restoreOnTaskFailed` or any other restore trigger.
- Does not modify `PlayerInteractor`, `TaskEventBus`, or `AudioManager`.
