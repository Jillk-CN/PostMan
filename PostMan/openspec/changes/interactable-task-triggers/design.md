# 设计：InteractableTaskTrigger 与 InteractableTaskAdvancer

## 架构

```
BaseTaskTrigger (MonoBehaviour)
    └── InteractableTaskTrigger
            ├── implements IInteractable
            └── ShouldTrigger() → true
                InteractWith(player) → TryTrigger()

BaseTaskAdvancer (MonoBehaviour)
    └── InteractableTaskAdvancer
            ├── implements IInteractable
            └── ShouldAdvance() → true
                InteractWith(player) → TryAdvance()
```

## 关键文件

| 文件 | 作用 |
|---|---|
| `Assets/Scripts/TaskSystem/Triggers/BaseTaskTrigger.cs` | 触发器基类，提供 `TryTrigger()` 和 `triggerOnce` 保护 |
| `Assets/Scripts/TaskSystem/Advancers/BaseTaskAdvancer.cs` | 推进器基类，提供 `TryAdvance()` 和 `advanceOnce` 保护 |
| `Assets/Scripts/Player/Interaction/IInteractable.cs` | 交互接口（`PostMan.Player` 命名空间） |
| `Assets/Scripts/TaskSystem/Triggers/InteractableTaskTrigger.cs` | **新建** — 交互式触发器 |
| `Assets/Scripts/TaskSystem/Advancers/InteractableTaskAdvancer.cs` | **新建** — 交互式推进器 |

## Inspector 字段

### InteractableTaskTrigger

继承自 `BaseTaskTrigger`：
- `targetTask`（TaskSO）— 要启动的任务
- `triggerOnce`（bool，默认 true）— 只触发一次

新增（交互设置）：
- `_canInteract`（bool，默认 true）— 是否允许交互
- `_priority`（int，默认 0）— 交互优先级

### InteractableTaskAdvancer

继承自 `BaseTaskAdvancer`：
- `targetTask`（TaskSO）— 要推进的任务
- `advanceAmount`（int，默认 1）— 每次推进量
- `advanceOnce`（bool，默认 false）— 只推进一次

新增（交互设置）：
- `_canInteract`（bool，默认 true）— 是否允许交互
- `_priority`（int，默认 0）— 交互优先级

## 使用示例

### 触发器

```
GameObject "TaskStarter"
├── InteractableTaskTrigger
│       Target Task: <TaskSO>
│       Trigger Once: ✓
│       Can Interact: ✓
│       Priority: 0
├── ShowInteractPrompt        ← 显示 E 键提示
└── LimitInteract (Priority: -1)  ← 可选：交互后禁用提示
```

### 推进器

```
GameObject "TaskAdvancer"
├── InteractableTaskAdvancer
│       Target Task: <TaskSO>
│       Advance Amount: 1
│       Advance Once: ✓
│       Can Interact: ✓
│       Priority: 0
└── ShowInteractPrompt
```

## 设计决策

**为何 ShouldTrigger/ShouldAdvance 返回 true？**
交互式组件的"条件"就是玩家主动按键，不需要额外的状态检查。
`PlayerInteractor` 在调用 `InteractWith` 前已通过 `CanInteract` 做了门控，
基类的 `triggerOnce`/`advanceOnce` 负责单次保护，职责清晰不重叠。
