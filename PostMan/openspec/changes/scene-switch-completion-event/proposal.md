# Proposal: 场景切换完成事件 (scene-switch-completion-event)

## What

在 `GameSceneManager` 中添加一个静态事件 `OnSceneSwitchCompleted`，在新场景全部加载完成且旧场景全部卸载完成后触发。

## Why

`GameSceneManager.SwitchScenes()` 完成"加载新场景 + 卸载旧场景"两个阶段后，没有向外部发布任何通知。

外部系统（UI 淡入淡出、任务系统、SceneInitializer）目前只能订阅 Unity 原生的 `SceneManager.sceneLoaded`，但该事件在加载完成时**立即**触发——此时旧场景尚未卸载，状态不干净。

需要一个在"加载 + 卸载"**全部完成**后才触发的通知点。

## Approach

在 `GameSceneManager` 类内部直接声明静态事件（项目体量小，无需单独的 EventBus 文件）。

事件在 `SwitchScenes` 的 `try` 块末尾（两阶段均成功完成后）触发；**异常路径不触发**，避免订阅者在不一致状态下运行。

触发时机：

```
SwitchScenes()
  ├── 阶段 1: 并行加载所有目标场景
  ├── 阶段 1.5: 重置输入 + 传送玩家
  ├── 阶段 2: 串行卸载旧场景
  └── ★ 触发 OnSceneSwitchCompleted(scenesToLoad)  ← 新增
```

事件参数使用 `IReadOnlyList<string>`，防止订阅者意外修改内部列表。
