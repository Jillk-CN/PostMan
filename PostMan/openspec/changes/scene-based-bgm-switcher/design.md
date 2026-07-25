# Design: Scene-Based BGM Switcher

## Overview

A single new `MonoBehaviour` — `SceneBGMSwitcher` — is added to `Assets/Scripts/BGM/SceneBGMSwitcher.cs`. It wires `GameSceneManager.OnSceneSwitchCompleted` to the same two-step AudioManager call used by `TaskBGMSwitcher` and `ColliderBGMSwitcher`: Stop then Play on the BGM track.

## Component Design

### `SceneBGMSwitcher : MonoBehaviour`

**File:** `Assets/Scripts/BGM/SceneBGMSwitcher.cs`
**Namespace:** global (consistent with `BGMPlayer`, `TaskBGMSwitcher`, `ColliderBGMSwitcher`)

#### Inspector Fields

```
[Header("场景触发配置")]
[SerializeField] string targetSceneAddress   // 目标场景的 Addressable key（与 SwitchScenes 中的 key 一致）
[SerializeField] bool   triggerOnce          // 是否只触发一次 (default: true)

[Header("Stop 参数")]
[SerializeField] bool  stopFadeOut          // 停止时是否淡出 (default: true)
[SerializeField] float stopFadeOutDuration  // 淡出时长 (default: 0.5f)

[Header("Play 参数")]
[SerializeField] AudioClip newClip         // 要播放的新音乐
[SerializeField] bool  loop                // 是否循环 (default: true)
[SerializeField] bool  playFadeIn          // 播放时是否淡入 (default: true)
[SerializeField] float playFadeInDuration  // 淡入时长 (default: 1f)
[SerializeField] float volume              // 音量 [Range(0,1)] (default: 1f)
```

**Note:** `trackId` is always `AudioTrackId.BGM` (hard-coded). No Inspector field is needed.

#### Runtime State

```
private bool _triggered = false;   // triggerOnce guard
```

#### Lifecycle

- `OnEnable` — subscribe `HandleSceneSwitchCompleted` to `GameSceneManager.OnSceneSwitchCompleted`
- `OnDisable` — unsubscribe `HandleSceneSwitchCompleted` from `GameSceneManager.OnSceneSwitchCompleted`

#### Core Logic

```
HandleSceneSwitchCompleted(IReadOnlyList<string> loadedScenes):
  if newClip == null → LogError with this context, return
  if targetSceneAddress is null or empty → LogError, return
  if triggerOnce and _triggered → return
  if loadedScenes does not contain targetSceneAddress → return
  _triggered = true
  AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)
  AudioManager.Instance.Play(AudioTrackId.BGM, newClip, loop, playFadeIn, playFadeInDuration, volume)
```

The `Contains` check uses `loadedScenes.Contains(targetSceneAddress)` for an exact address match.

## Dependencies & Integration

| Dependency | Location | How Used |
|---|---|---|
| `AudioManager` | `Assets/Scripts/AudioSystem/AudioManager.cs` | `Instance.Stop()` + `Instance.Play()` |
| `AudioTrackId` | `Assets/Scripts/AudioSystem/AudioTrackId.cs` | `AudioTrackId.BGM` constant |
| `GameSceneManager` | `Assets/Scripts/SceneManagement/GameSceneManager.cs` | `OnSceneSwitchCompleted` static event |
| `BGMPlayer` | `Assets/Scripts/BGM/BGMPlayer.cs` | Sibling file; no code dependency |
| `TaskBGMSwitcher` | `Assets/Scripts/BGM/TaskBGMSwitcher.cs` | Style/field parity reference |
| `ColliderBGMSwitcher` | `Assets/Scripts/BGM/ColliderBGMSwitcher.cs` | Style/field parity reference |

## Execution Flow

```
GameSceneManager.SwitchScenes() completes (load + unload phases done)
  → OnSceneSwitchCompleted?.Invoke(loadedSnapshot)
    → SceneBGMSwitcher.HandleSceneSwitchCompleted(loadedScenes)
      → newClip null guard passes
      → targetSceneAddress guard passes
      → triggerOnce guard: not yet triggered
      → loadedScenes.Contains(targetSceneAddress)? → yes
        → _triggered = true
        → AudioManager.Stop(BGM, fadeOut, duration)
        → AudioManager.Play(BGM, clip, loop, fadeIn, duration, volume)
```

## File Impact

| File | Action |
|---|---|
| `Assets/Scripts/BGM/SceneBGMSwitcher.cs` | **Create** (new script, ~80 lines) |

No existing files need modification.
