# Design: Debug Task Starter

## Architecture

```
Assets/Scripts/TaskSystem/
└── Debug/
    └── DebugTaskStarter.cs    (standalone MonoBehaviour — debug helper)
```

## Component Design

`DebugTaskStarter` is a plain MonoBehaviour with one serialized field:

```csharp
[SerializeField] private TaskSO[] _tasksToStart;
```

In `Awake`, it calls `TaskManager.Instance.StartTask(task)` for each entry. No other lifecycle methods are used.

## Key Design Decisions

### Enable/Disable Toggle
The component checks `enabled` implicitly — Unity does not call `Awake` on disabled MonoBehaviours... actually `Awake` is always called, but the component's enabled state is respected by wrapping the loop in a guard:

```csharp
private void Awake()
{
    if (!enabled) return;
    // start tasks
}
```

This means toggling the checkbox in the Inspector fully controls the behavior with no side-effects elsewhere.

### No Conditional Compilation
The component lives in the TaskSystem folder alongside production code. It is inert when disabled. Stripping debug helpers from release builds is left to the team's build pipeline (e.g., a `DEBUG` scripting define or manual removal). No `#if UNITY_EDITOR` guards are used to keep the script simple and predictable.

### Depends Only on TaskManager
The only external call is `TaskManager.Instance.StartTask(task)`. `TaskManager` is already a singleton that exists across all scenes (`DontDestroyOnLoad`). If `TaskManager.Instance` is null at `Awake` time (e.g., wrong scene load order), the call is silently skipped — no exception, no disruption.

## Data Flow

```
Scene Awake
  → DebugTaskStarter.Awake()
  → [foreach TaskSO in _tasksToStart]
  → TaskManager.Instance.StartTask(task)
  → TaskEventBus.PublishStarted(data)
  → TaskUI.HandleTaskStarted(data)     (normal downstream flow, unchanged)
```

Disabling the component short-circuits before any call is made — all downstream systems remain completely unaffected.
