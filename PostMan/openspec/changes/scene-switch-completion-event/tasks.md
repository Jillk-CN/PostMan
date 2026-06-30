# Tasks: 场景切换完成事件

## 实现步骤

- [x] **T1** 添加 `using System.Collections.ObjectModel;` 到 `GameSceneManager.cs` 顶部
- [x] **T2** 在"调试设置"区域之前声明 `public static event Action<IReadOnlyList<string>> OnSceneSwitchCompleted`
- [x] **T3** 在 `SwitchScenes()` 阶段 2（串行卸载）之后、`catch` 块之前，构建只读快照并触发事件
- [x] **T4** 创建 `openspec/changes/scene-switch-completion-event/proposal.md`
- [x] **T5** 创建 `openspec/changes/scene-switch-completion-event/design.md`
- [x] **T6** 创建 `openspec/changes/scene-switch-completion-event/tasks.md`

## 验证清单

- [ ] 在任意 MonoBehaviour 订阅 `OnSceneSwitchCompleted`，触发场景切换后确认日志在"所有旧场景卸载完成"之后打印
- [ ] 触发场景切换异常（如无效地址），确认 `OnSceneSwitchCompleted` **不**触发
- [ ] 确认事件参数为本次 `scenesToLoad` 的只读副本，修改副本不影响原列表
