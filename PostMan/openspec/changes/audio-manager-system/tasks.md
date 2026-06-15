# Tasks: 音频管理与播放系统

## 实现任务

### Phase 1 — AudioMixer Asset

- [ ] 在 `Assets/Audio/` 目录下创建 `MainMixer.mixer`，添加三个 Group：`Master`（根）、`BGM`（子）、`SFX`（子），并将 `BGMVolume`、`SFXVolume`、`MasterVolume` 三个参数 Expose 出去

### Phase 2 — 核心脚本

- [ ] 创建 `Assets/Scripts/AudioSystem/AudioTrackId.cs` — 枚举，包含 `MasterVolume / BGMVolume / SFXVolume`，每个值附 XML 注释说明用途
- [ ] 创建 `Assets/Scripts/AudioSystem/AudioTrackConfig.cs` — `[Serializable]` 配置类，字段：`AudioTrackId trackId`、`AudioMixerGroup mixerGroup`、`AudioSource audioSource`，附 `[Tooltip]` 说明
- [ ] 创建 `Assets/Scripts/AudioSystem/AudioManager.cs` — 继承 `MonoSingleton<AudioManager>`，实现：
  - `Init()` 中构建 `Dictionary<AudioTrackId, AudioTrackConfig>`，并从 PlayerPrefs 读取并应用初始音量
  - `Play(trackId, clip, loop, fadeIn, fadeInDuration, volume)` — 支持淡入
  - `Pause(trackId, fadeOut, fadeOutDuration)` — 支持淡出后暂停
  - `Stop(trackId, fadeOut, fadeOutDuration)` — 支持淡出后停止并清除 clip
  - `SetMasterVolume / SetBGMVolume / SetSFXVolume` — 写 Mixer + PlayerPrefs
  - 内部工具方法 `LinearToDecibels(float linear)` 处理 log 换算
  - 内部协程 `FadeCoroutine(AudioSource, float targetVol, float duration, Action onComplete)` 处理淡变，旧协程存入字典防止竞态

### Phase 3 — SettingsPanel 接入

- [ ] 修改 `Assets/Scripts/UI/Title/SettingsPanel.cs`：
  - 在 `BindControls()` 末尾追加三行：为 `sliderMasterVolume / sliderMusicVolume / sliderSFXVolume` 分别 `AddListener(OnMasterVolumeChanged)` 等
  - 在 `UnbindControls()` 末尾追加对应 `RemoveListener`
  - 在 `SyncUIFromPrefs()` 末尾追加三行：从 `PlayerPrefs` 读取音量值并 `SetValueWithoutNotify`
  - 在文件末尾（现有辅助方法区下方）新增三个私有方法：`OnMasterVolumeChanged / OnMusicVolumeChanged / OnSFXVolumeChanged`，各自调用 `AudioManager.Instance.SetXxxVolume(value)`
  - 移除 `BindControls()` 中的注释 `// 三个音量 Slider 暂不绑定逻辑`

### Phase 4 — 场景配置（Unity Editor 手动步骤）

- [ ] 在游戏主场景和标题场景中各添加空 GameObject `"AudioManager"`，挂载 `AudioManager` 脚本
- [ ] 在 `AudioManager` Inspector 的 `Tracks` 列表中添加三条配置，分别对应 `MasterVolume / BGMVolume / SFXVolume`，绑定对应 Mixer Group 和新建的 AudioSource 子对象
- [ ] 将 `MainMixer.mixer` 拖入 `AudioManager.mixer` 字段
- [ ] 在 `SettingsPanel` 预制体（或场景对象）的 Inspector 中确认三个音量 Slider 已正确赋值（原先已有 `[SerializeField]` 字段，只需检查引用不为空）

## 验证清单

- [ ] 进入标题场景，打开设置面板，拖动 Master / BGM / SFX Slider，确认游戏音量随之变化
- [ ] 关闭游戏重新打开，确认上次设置的音量值被正确恢复（UI 与实际音量一致）
- [ ] 调用 `AudioManager.Instance.Play(AudioTrackId.BGMVolume, clip, fadeIn: true, fadeInDuration: 1f)`，确认音频从静音淡入
- [ ] 调用 `AudioManager.Instance.Stop(AudioTrackId.BGMVolume, fadeOut: true, fadeOutDuration: 0.5f)`，确认音频淡出后停止
- [ ] 在 BGM Group 下新增一个子音轨，仅需修改 `AudioTrackId.cs` 和 Inspector 配置，确认无需改动 `AudioManager.cs` 其他代码
