# 设计：场景切换后玩家出生点传送

## 架构变更

```
SwitchScenes(load, unload, spawnPoint?)
├── 并行加载所有目标场景（不变）
├── 串行卸载旧场景（不变）
└── [新增] 若 spawnPoint 有值 → TeleportPlayer(spawnPoint.Value)

TeleportPlayer(Vector3 position)
├── PlayerInstance.Instance 获取玩家
├── CharacterController.enabled = false
├── transform.position = position
└── CharacterController.enabled = true
```

## 关键文件

| 文件 | 变更类型 | 说明 |
|---|---|---|
| `Assets/Scripts/SceneManagement/GameSceneManager.cs` | **修改** | `SwitchScenes` 增加 `Vector3? spawnPoint` 参数，新增 `TeleportPlayer` 私有方法 |
| `Assets/Scripts/SceneManagement/ColliderSceneChange.cs` | **修改** | 增加 `_spawnPoint` 字段，传递给 `SwitchScenes` |
| `Assets/Scripts/SceneManagement/InteractableSceneChange.cs` | **修改** | 增加 `_spawnPoint` 字段，传递给 `SwitchScenes` |
| `Assets/Scripts/Player/PlayerInstance.cs` | 只读参考 | `MonoSingleton<PlayerInstance>`，`PostMan.Player` 命名空间 |

## 接口变更

### GameSceneManager.SwitchScenes

```csharp
// 旧签名
public async void SwitchScenes(List<string> scenesToLoad, List<string> scenesToUnload)

// 新签名（spawnPoint 可选，null 表示不传送）
public async void SwitchScenes(List<string> scenesToLoad, List<string> scenesToUnload, Vector3? spawnPoint = null)
```

### TeleportPlayer（私有）

```csharp
// 通过 PlayerInstance 找到玩家，使用 CharacterController disable→set→enable 传送
private void TeleportPlayer(Vector3 position)
```

**CharacterController 传送必须 disable/enable**：直接赋值 `transform.position` 不会更新
CharacterController 内部碰撞状态，下一帧 `Move()` 会把玩家推回原位或穿墙。

### ColliderSceneChange / InteractableSceneChange

新增 Inspector 字段（放入「场景配置」分组）：

```csharp
[Tooltip("场景切换后玩家的出生坐标（留空则不传送）")]
[SerializeField] private Vector3 _spawnPoint;

[Tooltip("是否启用出生点传送（不勾选则忽略 _spawnPoint）")]
[SerializeField] private bool _useSpawnPoint = false;
```

调用时：

```csharp
GameSceneManager.Instance?.SwitchScenes(
    _scenesToLoad,
    _scenesToUnload,
    _useSpawnPoint ? _spawnPoint : null
);
```

## Inspector 分组

**ColliderSceneChange / InteractableSceneChange** — 「场景配置」分组新增：
- `_useSpawnPoint`（bool，默认 false）
- `_spawnPoint`（Vector3）

## 设计决策

**为何用 `Vector3?` 而非必填参数？**
保留向后兼容：旧的调用不传第三参数默认不传送，不影响已配置的场景触发器。

**为何用 `_useSpawnPoint` bool 而非 zero-check？**
`Vector3.zero` 是合法坐标（地图原点可能就是 0,0,0），用 bool 开关语义更清晰。

**传送时机：所有场景加载完成后、卸载开始前**
确保目标场景已存在于运行时（玩家不会出现在已卸载的空间），且旧场景尚未消失（避免闪烁）。
