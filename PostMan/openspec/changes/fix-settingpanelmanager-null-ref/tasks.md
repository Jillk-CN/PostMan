# 实现任务清单

## 任务

- [ ] **Task 1**: 修改 `Assets/SettingPanelManager.cs`
  - 在 `OnEnable()` 开头添加 null 检查：`var mgr = TitleUIManager.Instance; if (mgr == null) return;`
  - 用局部变量 `mgr` 替换两处 `TitleUIManager.Instance.` 调用
  - 在 `OnDisable()` 做同样的对称处理
  - 确保 `OnOpenSettingPanel` / `OnCloseSettingPanel` 方法对 `settingPanel` 也做 null 检查（防止 SerializeField 未赋值）

## 验收标准

- [ ] 在没有 `TitleUIManager` 的场景中启用 `SettingPanelManager` 不报错
- [ ] 在有 `TitleUIManager` 的场景中，设置面板的开关行为正常
- [ ] `OnDisable` 不抛出任何异常
