# Design: Update config.yaml with Current Project Context

## Target File

`openspec/config.yaml`

## Context Block Structure

The `context:` field is a freeform YAML multiline string. It will be structured as:

```
context: |
  [Tech Stack]
  [Implemented Systems]
  [Architecture Patterns]
  [Coding Conventions]
  [Domain Knowledge]
```

## Content Specification

### Tech Stack
- **Engine**: Unity 2022.3 LTS, Universal Render Pipeline (URP 14)
- **Language**: C# (.NET Standard 2.1)
- **Input**: Unity Input System (new)
- **UI Text**: TextMeshPro
- **Localization**: Unity Localization 1.5.3 (imported, not yet heavily used)
- **Post-processing**: URP Volume + custom ScriptableRendererFeature

### Implemented Systems

| System | Path | Status |
|--------|------|--------|
| Task System | `Assets/Scripts/TaskSystem/` | Complete |
| FSM Framework | `Assets/Scripts/FSM/` | Complete |
| Input Management | `Assets/Scripts/InputManagement/` | Complete |
| Player (Motion + Interaction) | `Assets/Scripts/Player/` | Complete |
| Interactable Objects | `Assets/Scripts/InteractableObjects/` | Complete |
| VHS Filter (URP post-process) | `Assets/Scripts/VHSFilter/` | Complete |
| UI Panels (Image/Text viewer) | `Assets/Scripts/UI/` | Complete |
| Subtitle System | `Assets/Scripts/Subtitle/` | Complete |
| Common Utilities | `Assets/Scripts/Common/` | Complete |

### Architecture Patterns
- **Event Bus**: `TaskEventBus` — static C# events, `internal` publish methods, subscribe in `OnEnable`/`OnDisable`
- **Singleton**: `MonoSingleton<T>` generic base (lazy `FindObjectOfType`, virtual `Init()`); also manual singletons
- **Immutable State**: `TaskRuntimeData` (sealed class, `WithProgress`/`WithStatus`), `VHSSettings` (struct, `With*` methods)
- **FSM**: `MonoFSM` + `FSMState` + `FSMCondition`; transitions via condition→stateID map
- **Interface-based Interaction**: `IInteractable` (CanInteract, Priority, `InteractWith`), `ISelectable`
- **Input Source Abstraction**: `IInputSource` + per-action sources managed by `GameInputManager`
- **URP Renderer Feature**: `ScriptableRendererFeature` + `ScriptableRenderPass` split

### Coding Conventions
- **Comments**: Simplified Chinese for all XML doc comments, `[Tooltip]`, `[Header]`, and inline `//` comments
- **XML docs**: `/// <summary>` on all public classes and methods
- **Inspector**: `[Header]` to group fields, `[Tooltip]` on key fields
- **Naming**: PascalCase classes/methods; `_camelCase` private fields (newer code); `camelCase` private fields (older code); interactable world objects prefixed `I_`
- **Namespaces**: `PostMan.Player`, `PostMan.InputManagement`, `PostMan.StateMachine`, `PostMan.Common`, `PostMan.UI`; TaskSystem and VHSFilter are in global namespace
- **File size**: Small focused files, typically 50–200 lines
- **Immutability**: Enforced by convention — state changes return new instances

### Domain Knowledge
- **Genre**: First-person exploration / narrative game (postal/delivery theme)
- **Player**: First-person CharacterController with walk/run FSM, raycast-based interaction, camera pitch control
- **Interaction model**: Player looks at objects → `PlayerDetector` highlights via `ISelectable` → player presses interact → `PlayerInteractor` calls `IInteractable.InteractWith` sorted by priority
- **Task/Quest**: Index-based tasks defined as ScriptableObjects; runtime state tracked by `TaskManager`; UI notified via `TaskEventBus`

## Rules Block

```yaml
rules:
  proposal:
    - Write proposals in English
    - Keep proposals under 400 words
    - Always include a "Non-goals" section
  design:
    - Write designs in English
    - Reference actual file paths from Assets/Scripts/
    - Follow existing architectural patterns (event bus, immutable state, interface-based)
  tasks:
    - Write tasks in English
    - Reference concrete file paths for each task
    - Include Unity Editor setup steps where relevant
    - Mark tasks as checkboxes (- [ ])
```
