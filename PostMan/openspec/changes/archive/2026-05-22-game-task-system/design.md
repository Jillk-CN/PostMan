# Design: Game Task System

## Architecture

```
Assets/Scripts/TaskSystem/
├── Data/
│   └── TaskSO.cs                  (ScriptableObject — static task definition)
├── Runtime/
│   ├── TaskStatus.cs              (Enum: Inactive/Active/Completed/Failed)
│   ├── TaskRuntimeData.cs         (Immutable runtime snapshot)
│   ├── TaskEventBus.cs            (Static event hub)
│   └── TaskManager.cs             (Singleton — owns all state)
├── Triggers/
│   ├── BaseTaskTrigger.cs         (Abstract base with triggerOnce guard)
│   └── ColliderTaskTrigger.cs     (Physics trigger + optional destroy)
├── Advancers/
│   ├── BaseTaskAdvancer.cs        (Abstract base with advanceOnce guard)
│   └── ColliderTaskAdvancer.cs    (Physics advancer + optional destroy)
└── UI/
    └── TaskUI.cs                  (Slide-in/out panel + Tab toggle)
```

## Key Design Decisions

### Immutable State
`TaskRuntimeData` is a sealed class with read-only properties. `WithProgress()` and `WithStatus()` return new instances. This prevents hidden mutation bugs when multiple systems hold references to the same state object.

### Static Event Bus
`TaskEventBus` is a static class with no MonoBehaviour dependency. UI subscribes in `OnEnable`, unsubscribes in `OnDisable`. Adding new observers (achievements, analytics) requires zero changes to existing code.

### Base Class Guards
`triggerOnce` and `advanceOnce` logic lives in the base classes, not in each concrete subclass. Every future trigger/advancer type gets this behavior for free.

### UI Animation
`TaskUI` uses a `Coroutine` with `SmoothStep` interpolation on `RectTransform.anchoredPosition`. No Animator component required. The panel anchors to the top-left; hidden position is `(-panelWidth, y)`, visible position is `(0, y)`.

## Data Flow

```
ColliderTaskTrigger.OnTriggerEnter
  → BaseTaskTrigger.TryTrigger()
  → TaskManager.StartTask(taskSO)
  → creates TaskRuntimeData (immutable)
  → TaskEventBus.PublishStarted(data)
  → TaskUI.HandleTaskStarted(data) → SlideIn() + UpdateTexts()

ColliderTaskAdvancer.OnTriggerEnter
  → BaseTaskAdvancer.TryAdvance()
  → TaskManager.AdvanceTask(index, amount)
  → old.WithProgress(newProgress) → new snapshot
  → if complete: PublishCompleted → TaskUI.SlideOut()
  → else: PublishAdvanced → TaskUI.UpdateTexts()
```

## Extension Pattern

```csharp
// Custom trigger example
public class InteractTaskTrigger : BaseTaskTrigger
{
    protected override bool ShouldTrigger() =>
        Input.GetKeyDown(KeyCode.E);

    private void Update() => TryTrigger();
}
```
