# 设计：离开触发区域时恢复原 BGM

## 概述

本次改动涉及两个文件：

1. `AudioManager.cs` — 新增 `GetClip(AudioTrackId)` 公开方法
2. `ColliderBGMSwitcher.cs` — 新增私有字段 `_previousClip` / `_previousVolume` 与 `OnTriggerExit` 方法，并在 `OnTriggerEnter` 中增加快照逻辑

## AudioManager 修改

### 新增方法：`GetClip`

**位置：** 「公开 API：播放控制」区块末尾（`Stop` 方法之后）

```csharp
/// <summary>
/// 获取指定音轨当前加载的 AudioClip。
/// 若音轨未配置或 AudioSource 为空则返回 null。
/// </summary>
/// <param name="trackId">目标音轨 ID</param>
/// <returns>当前 AudioClip，未播放时也可能为 null</returns>
public AudioClip GetClip(AudioTrackId trackId)
{
    if (!TryGetSource(trackId, out AudioSource source)) return null;
    return source.clip;
}
```

**说明：**
- 直接读取 `AudioSource.clip`，不涉及任何状态变更，完全只读。
- 利用现有的 `TryGetSource` 保持一致的错误处理路径。

## ColliderBGMSwitcher 修改

### 新增运行时字段

在「运行时状态」区块，紧跟 `_triggered` 之后：

```csharp
/// <summary>进入触发区域前正在播放的 BGM 片段快照，退出时用于还原。</summary>
private AudioClip _previousClip;

/// <summary>进入触发区域前 BGM 音轨的音量快照。</summary>
private float _previousVolume = 1f;
```

### OnTriggerEnter — 增加快照

在调用 `AudioManager.Instance.Stop` **之前**插入两行快照：

```csharp
// 快照当前 BGM，供退出时还原
_previousClip   = AudioManager.Instance.GetClip(AudioTrackId.BGM);
_previousVolume = /* 通过 GetClip 后再取 source.volume，
                     但 AudioManager 未暴露 volume；
                     此处统一记为 1f，与 Play 默认值一致 */
                  1f;
```

> **注意：** `AudioManager` 未暴露 source 的当前 volume，只新增了 `GetClip`。`_previousVolume` 固定为 `1f`（与 `Play` 默认参数一致）。若未来需要精确还原音量，可再新增 `GetVolume` 方法，但超出本次范围。

### 新增方法：OnTriggerExit

```
OnTriggerExit(Collider other):
  Tag 过滤：triggerTag 非空且 other.tag != triggerTag → return
  triggerOnce 且未触发过（_triggered == false）→ return   // 从未进入过，无需还原
  Stop(BGM, stopFadeOut, stopFadeOutDuration)
  if _previousClip != null:
    Play(BGM, _previousClip, loop=true, playFadeIn, playFadeInDuration, _previousVolume)
```

**参数复用说明：**

| 参数 | 退出时用途 |
|---|---|
| `stopFadeOut` / `stopFadeOutDuration` | 退出时淡出当前 BGM（与进入时 Stop 行为对称） |
| `playFadeIn` / `playFadeInDuration` | 退出后淡入还原曲目 |
| `loop` | 还原曲目是否循环（原曲目通常是循环的） |

## 执行流程

```
玩家进入触发区域
  → OnTriggerEnter
    → 快照：_previousClip = GetClip(BGM)
    → Stop(BGM, fadeOut)
    → Play(BGM, newClip, ...)
    → _triggered = true

玩家离开触发区域
  → OnTriggerExit
    → Tag 过滤 / triggerOnce 未触发保护
    → Stop(BGM, fadeOut)
    → Play(BGM, _previousClip, ...)  ← 若 _previousClip == null 则跳过 Play
```

## 文件影响

| 文件 | 操作 |
|---|---|
| `Assets/Scripts/AudioSystem/AudioManager.cs` | **修改** — 新增 `GetClip` 方法（约 8 行） |
| `Assets/Scripts/BGM/ColliderBGMSwitcher.cs` | **修改** — 新增 2 个字段 + 快照逻辑 + `OnTriggerExit` 方法（约 20 行） |
