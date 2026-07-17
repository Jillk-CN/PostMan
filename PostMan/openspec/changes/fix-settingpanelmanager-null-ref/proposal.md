# 修复 SettingPanelManager NullReferenceException

## 问题描述

```
NullReferenceException: Object reference not set to an instance of an object
SettingPanelManager.OnEnable () (at Assets/SettingPanelManager.cs:14)
```

## 根本原因

`SettingPanelManager.OnEnable()` 在第 14 行调用 `TitleUIManager.Instance`，
但 `MonoSingleton<T>.Instance` 在场景中找不到对应对象时会返回 `null`（而非自动创建）。

两种触发场景：
1. **执行顺序问题**：`SettingPanelManager.OnEnable()` 先于 `TitleUIManager.Awake()` 执行，`Instance` 尚未初始化。
2. **场景缺失**：当前场景根本没有挂载 `TitleUIManager` 的 GameObject。

## 修复方案

在 `SettingPanelManager` 的 `OnEnable` / `OnDisable` 中增加 null 检查，
仅在 `TitleUIManager.Instance` 不为 null 时才注册/注销监听器。

**不修改 `MonoSingleton` 或 `TitleUIManager`**，变更范围最小，影响最低。

## 影响范围

- `Assets/SettingPanelManager.cs`（唯一修改文件）
