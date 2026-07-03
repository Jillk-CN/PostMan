## Proposal: Debug Task Starter

### What

A single, self-contained MonoBehaviour (`DebugTaskStarter`) that calls `TaskManager.StartTask()` for a configurable list of `TaskSO` assets at the start of the scene (`Awake`). The component can be enabled or disabled in the Inspector to toggle the debug shortcut without modifying any other code.

### Why

When iterating on a specific task mid-flow, testers must replay all preceding tasks to reach it — wasting time on already-validated content. A lightweight debug helper that activates chosen tasks at startup lets testers jump directly to the task under test, without touching production logic.

### Non-goals

- No auto-advancing or auto-completing tasks (only starts them).
- No editor-only compilation guards (the component is inert when disabled; stripping it from builds is a separate concern).
- No dependency on any system other than `TaskManager`.
- No UI, no events, no configuration files — pure Inspector-driven setup.

### Approach

A single MonoBehaviour script placed under `Assets/Scripts/TaskSystem/Debug/`. In `Awake`, if the component is enabled, it iterates its `TaskSO[]` array and calls `TaskManager.Instance.StartTask(task)` for each entry. Because it only calls the public `TaskManager` API, enabling or disabling the component has zero side-effects on any other system.
