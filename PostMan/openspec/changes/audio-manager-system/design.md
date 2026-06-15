# Design: 音频管理与播放系统

## 架构总览

```
Assets/
├── Audio/
│   └── MainMixer.mixer          — Unity AudioMixer Asset（含 Master/BGM/SFX 三个 Group）
│
└── Scripts/
    └── AudioSystem/
        ├── AudioTrackId.cs       — 枚举：预定义音轨 ID（MasterVolume/BGMVolume/SFXVolume）
        ├── AudioTrackConfig.cs   — [Serializable] 配置项：trackId + AudioMixerGroup + AudioSource 引用
        ├── AudioManager.cs       — MonoSingleton<AudioManager>：音轨注册表 + Play/Pause/Stop API
        └── AudioFadeHandle.cs    — 淡入淡出协程封装（内部工具类，不对外暴露）
```

修改文件：
- `Assets/Scripts/UI/Title/SettingsPanel.cs` — 补全三个音量 Slider 的绑定与同步逻辑

## AudioMixer 层级结构

```
Master (Group: MasterVolume)
├── BGM    (Group: BGMVolume)     — exposed param: "BGMVolume"
│   └── [子音轨由调用方在 Inspector 中绑定到此 Group]
└── SFX    (Group: SFXVolume)     — exposed param: "SFXVolume"
    └── [子音轨由调用方在 Inspector 中绑定到此 Group]
```

Exposed parameter 命名规则：`MasterVolume` / `BGMVolume` / `SFXVolume`。
音量换算：Slider 值域 0~1 → `Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f`（dB），避免 log(0)。

## 关键设计决策

### 音轨注册表
`AudioManager` 内部维护 `Dictionary<AudioTrackId, AudioTrackConfig>` 作为运行时注册表，由 Inspector 配置的 `List<AudioTrackConfig>` 在 `Init()` 时填充。扩展新音轨只需在 Inspector 列表追加配置，核心代码零改动。

### 淡入淡出
`AudioFadeHandle` 是私有嵌套类（或同文件内部类），封装一个正在进行的淡变协程。Play 时若指定 `fadeIn = true`，先将音量设为 0 再逐帧插值到目标音量；Pause/Stop 时若指定 `fadeOut = true`，先插值到 0 再执行暂停/停止。同一音轨上的新淡变会先终止旧协程，避免竞态。

### 子音轨音量控制
子音轨（BGM/SFX 下的扩展音轨）绑定到父 Group，其 AudioSource 的 `volume` 属性由 `Play(trackId, clip, volume:...)` 直接设置。整体音量由 Mixer Group 的 exposed parameter 控制，两者相乘形成最终输出音量，无需在 AudioManager 中单独保存子音轨音量状态。

### SettingsPanel 接入
`SettingsPanel` 仅在以下三处做最小化修改，不触碰其他任何方法：
1. `BindControls()` — 为三个音量 Slider 追加 `AddListener`
2. `UnbindControls()` — 对应 `RemoveListener`
3. `SyncUIFromPrefs()` — 从 PlayerPrefs 读取音量值并 `SetValueWithoutNotify`

PlayerPrefs 键名常量定义在 `AudioManager` 中（`KeyMasterVolume` 等），`SettingsPanel` 通过调用 `AudioManager.Instance.SetMasterVolume(value)` 等方法设置，由 `AudioManager` 内部负责写 PlayerPrefs，职责分离。

## 数据流

```
SettingsPanel.sliderMasterVolume.onValueChanged
  → AudioManager.SetMasterVolume(value)
  → mixer.SetFloat("MasterVolume", LinearToDecibels(value))
  → PlayerPrefs.SetFloat(KeyMasterVolume, value)

AudioManager.Init()
  → 读取 PlayerPrefs 音量值（默认 1f）
  → mixer.SetFloat(...)  同步 Mixer
  → [注册音轨字典]

SettingsPanel.OnEnable → SyncUIFromPrefs()
  → sliderMasterVolume.SetValueWithoutNotify(PlayerPrefs.GetFloat(KeyMasterVolume, 1f))
  → （BGM / SFX 同理）

外部调用示例：
AudioManager.Instance.Play(AudioTrackId.BGMVolume, bgmClip, loop: true, fadeIn: true, fadeInDuration: 1f)
AudioManager.Instance.Pause(AudioTrackId.BGMVolume, fadeOut: true, fadeOutDuration: 0.5f)
AudioManager.Instance.Stop(AudioTrackId.BGMVolume, fadeOut: true, fadeOutDuration: 0.5f)
```

## 公开 API 签名

```csharp
// 播放
void Play(AudioTrackId trackId, AudioClip clip,
          bool loop = false,
          bool fadeIn = false, float fadeInDuration = 0.5f,
          float volume = 1f);

// 暂停
void Pause(AudioTrackId trackId,
           bool fadeOut = false, float fadeOutDuration = 0.5f);

// 停止并清除
void Stop(AudioTrackId trackId,
          bool fadeOut = false, float fadeOutDuration = 0.5f);

// 主音轨音量（由 SettingsPanel 调用）
void SetMasterVolume(float linearValue);
void SetBGMVolume(float linearValue);
void SetSFXVolume(float linearValue);
```

## 扩展子音轨方式

1. 在 `MainMixer.mixer` 中在 BGM 或 SFX Group 下添加子 Group（如 `BGM_Ambient`）
2. 在场景 `AudioManager` 的 Inspector `Tracks` 列表中追加一项：
   - `trackId`：从 `AudioTrackId` 枚举中选择（或新增枚举值后重新编译）
   - `mixerGroup`：指向刚创建的 Mixer Group
   - `audioSource`：场景中对应的 AudioSource 组件
3. 完成，调用 `AudioManager.Instance.Play(新TrackId, clip)` 即可

> 注意：若需要新增枚举值，仅修改 `AudioTrackId.cs` 一个文件即可，其余代码无需改动。
