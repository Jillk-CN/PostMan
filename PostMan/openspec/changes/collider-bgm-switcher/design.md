# Design: Collider-Triggered BGM Switcher

## Overview

A single new `MonoBehaviour` — `ColliderBGMSwitcher` — is added to `Assets/Scripts/BGM/ColliderBGMSwitcher.cs`. It wires `OnTriggerEnter` to the same two-step AudioManager call used by `TaskBGMSwitcher`: Stop then Play on the BGM track.

## Component Design

### `ColliderBGMSwitcher : MonoBehaviour`

**File:** `Assets/Scripts/BGM/ColliderBGMSwitcher.cs`
**Namespace:** global (consistent with `BGMPlayer` and `TaskBGMSwitcher`)

#### Inspector Fields

```
[Header("触发配置")]
[SerializeField] string triggerTag       // 过滤进入物体的 Tag (default: "Player")
[SerializeField] bool   triggerOnce      // 是否只触发一次 (default: true)

[Header("Stop 参数")]
[SerializeField] bool  stopFadeOut          // 停止时是否淡出 (default: true)
[SerializeField] float stopFadeOutDuration  // 淡出时长 (default: 0.5f)

[Header("Play 参数")]
[SerializeField] AudioClip newClip         // 要播放的新音乐
[SerializeField] bool  loop                // 是否循环 (default: true)
[SerializeField] bool  playFadeIn          // 播放时是否淡入 (default: true)
[SerializeField] float playFadeInDuration  // 淡入时长 (default: 1f)
[SerializeField] float volume              // 音量 (default: 1f)
```

**Note:** `trackId` is always `AudioTrackId.BGM` (hard-coded). No Inspector field is needed.

#### Runtime State

```
private bool _triggered = false;   // triggerOnce guard
```

#### Core Logic

```
OnTriggerEnter(Collider other):
  if newClip == null → LogError, return
  if triggerTag is non-empty and other.tag != triggerTag → return
  if triggerOnce and _triggered → return
  _triggered = true
  AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)
  AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)
```

A null guard on `newClip` logs a `Debug.LogError` with `this` as context and returns early.
`triggerTag` can be set to an empty string to accept any collider.

## Dependencies & Integration

| Dependency | Location | How Used |
|---|---|---|
| `AudioManager` | `Assets/Scripts/AudioSystem/AudioManager.cs` | `Instance.Stop()` + `Instance.Play()` |
| `AudioTrackId` | `Assets/Scripts/AudioSystem/AudioTrackId.cs` | `AudioTrackId.BGM` constant |
| `BGMPlayer` | `Assets/Scripts/BGM/BGMPlayer.cs` | Sibling file; no code dependency |
| `TaskBGMSwitcher` | `Assets/Scripts/BGM/TaskBGMSwitcher.cs` | Style/field parity reference |

## Execution Flow

```
Player enters trigger collider
  → ColliderBGMSwitcher.OnTriggerEnter(other)
    → tag filter passes?  → yes
    → triggerOnce guard?  → not yet triggered
      → _triggered = true
      → AudioManager.Stop(BGM, fadeOut, duration)
      → AudioManager.Play(BGM, clip, loop, fadeIn, duration, volume)
```

## Scene Setup Requirements

- The GameObject holding `ColliderBGMSwitcher` must also have a `Collider` component with **Is Trigger** checked.
- The player GameObject must have a `Rigidbody` (or `Rigidbody2D`) for `OnTriggerEnter` to fire.
- The player's tag must match `triggerTag` (default `"Player"`).

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/BGM/ColliderBGMSwitcher.cs` | **Create** (new script, ~80 lines) |

No existing files need modification.
