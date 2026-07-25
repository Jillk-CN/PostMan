# 方案：离开触发区域时恢复原 BGM（ColliderBGMSwitcher 退出还原）

## 问题

`ColliderBGMSwitcher` 目前只处理进入（`OnTriggerEnter`）时的 BGM 切换，玩家离开触发区域后 BGM 不会自动恢复。这对于「区域 BGM」的常见设计场景不够完整——玩家走出区域后应当恢复进入前正在播放的音乐。

`AudioManager` 目前没有对外暴露「获取指定音轨当前正在播放的 AudioClip」的方法，无法在 `OnTriggerEnter` 时提前快照原曲目。

## 解决方案

分两步：

1. **在 `AudioManager` 中新增 `GetClip(AudioTrackId)` 方法**，返回指定音轨 `AudioSource` 当前的 `clip`（可为 `null`）。这是最小侵入式的 API 扩展，不改变任何现有逻辑。

2. **在 `ColliderBGMSwitcher` 中增加 `OnTriggerExit` 逻辑**：
   - 在 `OnTriggerEnter` 成功切换前，先通过 `AudioManager.Instance.GetClip(BGM)` 快照当前 BGM 到 `_previousClip`，同时记录当前音量到 `_previousVolume`。
   - `OnTriggerExit` 时，停止当前 BGM 并用 `_previousClip` 恢复播放（若 `_previousClip` 为 `null` 则只 Stop 不 Play）。
   - 退出参数（是否淡出/淡入、时长）复用现有的 Stop/Play 参数，不另起一套字段以保持 Inspector 简洁。
   - `triggerOnce = true` 时，退出时仍执行还原（已进入过才能退出），但之后再进入不再触发。

## 关键决策

- **`GetClip` 而非暴露 `AudioSource`** — 最小暴露原则，调用方只需要 clip，不需要控制 source。
- **快照时机在切换前** — 保证记录的是切换之前的曲目，而不是切换之后的 `newClip`。
- **`_previousVolume` 一并快照** — `AudioSource.volume` 在淡变过程中可能不准确，但快照进入前的初始音量已足够满足还原需求。
- **`_previousClip == null` 时只 Stop** — 进入区域前可能本来就没有 BGM，此时退出只停止当前音乐，不播放任何内容。
- **退出时不重置 `_triggered`** — 与原 `triggerOnce` 语义保持一致：「只触发一次进入」。

## 非目标

- 不支持嵌套触发区域（多个 ColliderBGMSwitcher 叠加时的栈式恢复）。
- 不修改 `TaskBGMSwitcher`。
- 不新增任何 Inspector 字段（退出参数复用现有字段）。
