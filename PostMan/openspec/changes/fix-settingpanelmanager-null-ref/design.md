# 设计方案

## 现状

```csharp
private void OnEnable()
{
    // 若 TitleUIManager.Instance 为 null，此处直接抛出 NullReferenceException
    TitleUIManager.Instance.OpenSettingsEvent.AddListener(OnOpenSettingPanel);
    TitleUIManager.Instance.CloseSettingsEvent.AddListener(OnCloseSettingPanel);
}
```

`MonoSingleton<T>` 在找不到对象时返回 `null`，调用方未做防护。

## 修复后

```csharp
private void OnEnable()
{
    var mgr = TitleUIManager.Instance;
    if (mgr == null) return;          // 防止场景中无 TitleUIManager 时崩溃
    mgr.OpenSettingsEvent.AddListener(OnOpenSettingPanel);
    mgr.CloseSettingsEvent.AddListener(OnCloseSettingPanel);
}

private void OnDisable()
{
    var mgr = TitleUIManager.Instance;
    if (mgr == null) return;          // 对称保护，防止 OnDisable 时同样崩溃
    mgr.OpenSettingsEvent.RemoveListener(OnOpenSettingPanel);
    mgr.CloseSettingsEvent.RemoveListener(OnCloseSettingPanel);
}
```

## 设计决策

| 方案 | 说明 | 选择原因 |
|------|------|---------|
| **Null 检查（本方案）** | OnEnable/OnDisable 判空后跳过 | 改动最小，不破坏现有架构 |
| 修改 MonoSingleton 自动创建 | 注释掉的代码改为自动实例化 | 副作用大，可能掩盖其他问题 |
| 使用 Start 替代 OnEnable | 延迟到 Start 再订阅 | 可行但改变了脚本语义，且不能处理 disable/re-enable 场景 |

## 不涉及改动

- `MonoSingleton.cs`
- `TitleUIManager.cs`
- 任何场景文件或 Prefab
