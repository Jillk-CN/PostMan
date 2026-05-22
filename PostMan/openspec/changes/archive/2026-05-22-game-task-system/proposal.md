## Proposal: Game Task System

### What

Implement a modular, event-driven task system for the PostMan Unity game. The system consists of five components: TaskSO (data asset), TaskManager (singleton), TaskEventBus (decoupled event hub), BaseTaskTrigger/BaseTaskAdvancer (extensible base classes with concrete collider implementations), and TaskUI (animated HUD panel).

### Why

The game needs a reusable task/quest system that can be wired up entirely in the Unity Inspector without writing new code for each task. The architecture must be extensible — new trigger and advancer types should be addable by subclassing, not by modifying existing files.

### Non-goals

- Save/load persistence (out of scope for this iteration)
- Task prerequisites or chaining (can be added later via TaskEventBus subscribers)
- Multiple simultaneous task UI panels

### Approach

Event-bus architecture (Approach B): TaskManager owns all runtime state as immutable snapshots. All state changes are broadcast through a static TaskEventBus. UI and any future observers subscribe to the bus — zero coupling to TaskManager.
