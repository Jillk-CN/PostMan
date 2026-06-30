# Design: 场景切换完成事件

## 修改文件

`Assets/Scripts/SceneManagement/GameSceneManager.cs`（唯一改动点）

## 详细设计

### 新增 using

```csharp
using System.Collections.ObjectModel;
```

`List<T>.AsReadOnly()` 返回 `ReadOnlyCollection<T>`，位于 `System.Collections.ObjectModel` 命名空间。

### 事件声明（放在"调试设置"区域之前）

```csharp
// ─────────────────────────────────────────────
// 公开事件
// ─────────────────────────────────────────────

/// <summary>
/// 场景切换完成事件：新场景全部加载完成且旧场景全部卸载完成后触发。
/// 参数为本次加载的场景地址列表（只读副本）。
/// </summary>
public static event Action<IReadOnlyList<string>> OnSceneSwitchCompleted;
```

### 事件触发（SwitchScenes try 块末尾，阶段 2 之后）

```csharp
// ── 阶段 3：通知外部系统场景切换已全部完成 ──
var loadedSnapshot = scenesToLoad != null
    ? (IReadOnlyList<string>)scenesToLoad.AsReadOnly()
    : Array.Empty<string>();
OnSceneSwitchCompleted?.Invoke(loadedSnapshot);
Log("场景切换完成事件已触发。");
```

## 设计约束

| 约束 | 说明 |
|------|------|
| 不修改现有方法签名 | `SwitchScenes` 参数列表不变 |
| 只读参数 | `IReadOnlyList<string>` 防止订阅者修改内部状态 |
| 异常路径不触发 | 事件在 `try` 块末尾，`catch` 之前，保证状态干净 |
| 无新文件 | 仅修改 `GameSceneManager.cs` |
| 中文注释 | 与现有代码风格一致 |

## 订阅示例

```csharp
private void OnEnable()
{
    GameSceneManager.OnSceneSwitchCompleted += HandleSceneSwitchCompleted;
}

private void OnDisable()
{
    GameSceneManager.OnSceneSwitchCompleted -= HandleSceneSwitchCompleted;
}

private void HandleSceneSwitchCompleted(IReadOnlyList<string> loadedScenes)
{
    Debug.Log($"场景切换完成，已加载场景：{string.Join(", ", loadedScenes)}");
}
```
