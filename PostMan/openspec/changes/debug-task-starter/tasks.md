# Tasks: Debug Task Starter

## Implementation Tasks

- [x] Create `Assets/Scripts/TaskSystem/Debug/DebugTaskStarter.cs`
  - MonoBehaviour with `[SerializeField] private TaskSO[] _tasksToStart`
  - `Awake()` returns early if `!enabled`, then iterates `_tasksToStart` and calls `TaskManager.Instance.StartTask(task)` for each non-null entry
  - XML doc comments in Simplified Chinese on the class and `Awake` method
  - `[Header]` and `[Tooltip]` on the serialized field

## Setup Checklist (Unity Editor)

- [ ] Add an empty GameObject to the test scene (e.g., "DebugTaskStarter")
- [ ] Attach `DebugTaskStarter` component
- [ ] Assign the `TaskSO` assets for tasks you want pre-started in the `Tasks To Start` array
- [ ] Enable the component to activate debug behavior; disable to run the scene normally
