# 提案：InteractableTaskTrigger 与 InteractableTaskAdvancer

## 背景

任务系统已有基于物理碰撞的 `ColliderTaskTrigger` 和 `ColliderTaskAdvancer`，
但游戏中大量场景需要玩家主动按 E 键交互才触发任务，而非走入碰撞区域。

## 目标

通过最小代码量，桥接任务系统与交互系统：
- 继承现有任务基类（`BaseTaskTrigger` / `BaseTaskAdvancer`）
- 同时实现 `IInteractable` 接口
- 复用基类的单次触发/推进保护逻辑，不重复实现

## 方案

新建两个组件类：

| 类名 | 继承 | 接口 | 触发时机 |
|---|---|---|---|
| `InteractableTaskTrigger` | `BaseTaskTrigger` | `IInteractable` | 玩家按 E 键 → `InteractWith()` → `TryTrigger()` |
| `InteractableTaskAdvancer` | `BaseTaskAdvancer` | `IInteractable` | 玩家按 E 键 → `InteractWith()` → `TryAdvance()` |

两个类的 `ShouldTrigger()` / `ShouldAdvance()` 均返回 `true`，
条件判断交由玩家的交互时机决定，单次保护由基类 `triggerOnce` / `advanceOnce` 处理。

## 不在本次范围内

- 修改 `BaseTaskTrigger` / `BaseTaskAdvancer` 基类
- 修改 `IInteractable` 接口或 `PlayerInteractor`
- 新增 UI 提示组件（使用现有 `ShowInteractPrompt`）
