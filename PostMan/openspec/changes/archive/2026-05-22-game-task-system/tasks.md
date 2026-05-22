# Tasks: Game Task System

## Implementation Tasks

- [x] Create `Assets/Scripts/TaskSystem/Runtime/TaskStatus.cs` — enum with Inactive/Active/Completed/Failed
- [x] Create `Assets/Scripts/TaskSystem/Data/TaskSO.cs` — ScriptableObject with taskIndex, taskName, taskDescription, totalProgress
- [x] Create `Assets/Scripts/TaskSystem/Runtime/TaskRuntimeData.cs` — immutable sealed class with WithProgress/WithStatus/ProgressRatio
- [x] Create `Assets/Scripts/TaskSystem/Runtime/TaskEventBus.cs` — static event hub with 4 events and internal publish methods
- [x] Create `Assets/Scripts/TaskSystem/Runtime/TaskManager.cs` — singleton with StartTask/AdvanceTask/FailTask/GetTask/IsTaskActive/IsTaskCompleted
- [x] Create `Assets/Scripts/TaskSystem/Triggers/BaseTaskTrigger.cs` — abstract base with triggerOnce guard and TryTrigger()
- [x] Create `Assets/Scripts/TaskSystem/Triggers/ColliderTaskTrigger.cs` — OnTriggerEnter + destroyOnTrigger option
- [x] Create `Assets/Scripts/TaskSystem/Advancers/BaseTaskAdvancer.cs` — abstract base with advanceOnce guard and TryAdvance()
- [x] Create `Assets/Scripts/TaskSystem/Advancers/ColliderTaskAdvancer.cs` — OnTriggerEnter + destroyOnAdvance option
- [x] Create `Assets/Scripts/TaskSystem/UI/TaskUI.cs` — slide-in/out animation + Tab toggle + TMP text updates

## Setup Checklist (Unity Editor)

- [ ] Create a TaskSO asset: right-click in Project → Create → TaskSystem → Task
- [ ] Add empty GameObject "TaskManager" to scene, attach TaskManager script
- [ ] Add Collider (Is Trigger) to trigger object, attach ColliderTaskTrigger, assign TaskSO
- [ ] Add Collider (Is Trigger) to advancer object, attach ColliderTaskAdvancer, assign TaskSO
- [ ] Create Canvas → Panel (anchor: top-left), add TMP_Text children, attach TaskUI, assign references
