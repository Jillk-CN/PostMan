# 任务列表：场景切换后玩家出生点传送

## 任务

- [ ] 修改 `Assets/Scripts/SceneManagement/GameSceneManager.cs`
  - `SwitchScenes` 签名增加 `Vector3? spawnPoint = null` 第三参数
  - 所有加载完成后（`Task.WhenAll` 之后）调用私有方法 `TeleportPlayer`
  - 新增 `TeleportPlayer(Vector3 position)`：通过 `PlayerInstance.Instance` 获取玩家，`CharacterController.enabled = false` → `transform.position = position` → `CharacterController.enabled = true`
  - `using PostMan.Player` 引用 `PlayerInstance`

- [ ] 修改 `Assets/Scripts/SceneManagement/ColliderSceneChange.cs`
  - 「场景配置」Header 下新增 `_useSpawnPoint`（bool，默认 false）和 `_spawnPoint`（Vector3）字段
  - `OnTriggerEnter` 中传递 `_useSpawnPoint ? _spawnPoint : (Vector3?)null` 给 `SwitchScenes`

- [ ] 修改 `Assets/Scripts/SceneManagement/InteractableSceneChange.cs`
  - 「场景配置」Header 下新增 `_useSpawnPoint`（bool，默认 false）和 `_spawnPoint`（Vector3）字段
  - `InteractWith` 中传递 `_useSpawnPoint ? _spawnPoint : (Vector3?)null` 给 `SwitchScenes`

## 完成标准

- [ ] Unity 编译无报错
- [ ] `ColliderSceneChange` / `InteractableSceneChange` Inspector 中「场景配置」分组显示 `UseSpawnPoint` 和 `SpawnPoint` 字段
- [ ] `_useSpawnPoint` 不勾选时，场景切换后玩家位置不变（旧行为保留）
- [ ] `_useSpawnPoint` 勾选并设置坐标时，场景切换加载完成后玩家被传送到指定坐标
- [ ] 传送后玩家移动正常（CharacterController 状态未损坏）
