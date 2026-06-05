# 提案：场景切换后玩家出生点传送

## 背景

`GameSceneManager.SwitchScenes()` 当前只负责 Addressables 场景的加载与卸载，
不处理玩家位置。场景切换后玩家仍停留在旧坐标，若新场景的地图布局与旧场景不同，
玩家会出现在错误位置，导致穿墙或悬空。

## 目标

在 `SwitchScenes` 中增加一个可选的 `Vector3 spawnPoint` 参数，
场景加载完成后自动将玩家传送到指定坐标，解决切换后位置错误的问题。

## 方案

1. `GameSceneManager.SwitchScenes` 增加 `Vector3? spawnPoint = null` 可选参数
2. 所有场景加载完成后，若 `spawnPoint` 有值，通过 `PlayerInstance.Instance` 找到玩家，
   使用 `CharacterController` disable→set position→enable 三步传送
3. `ColliderSceneChange` 和 `InteractableSceneChange` 各增加 `_spawnPoint` 字段，
   传递给 `SwitchScenes`

## 不在本次范围内

- 新增 SpawnPoint GameObject / 预设点管理系统
- 修改 `PlayerMotion`、`PlayerInteractor` 等玩家子系统
- 场景过渡动画（黑屏/淡入淡出）
