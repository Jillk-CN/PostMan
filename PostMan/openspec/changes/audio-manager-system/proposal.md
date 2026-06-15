## Proposal: 音频管理与播放系统

### What

为 PostMan 游戏实现一套集中式音频管理系统，基于 Unity AudioMixer 构建分层音轨结构，并提供统一的播放 / 暂停 / 清除 API，同时将三条主音轨的音量控制接入现有设置面板。

### Why

游戏当前音频完全分散在各个 Interactable 和 UI 脚本中，每个对象自挂 AudioSource，无法统一控制全局音量，也无法实现淡入淡出等效果。`SettingsPanel` 已预留了三个音量 Slider（`sliderMasterVolume` / `sliderMusicVolume` / `sliderSFXVolume`）但逻辑空置，需要接入。

### Non-goals

- 3D 空间音效定位（各交互对象自挂 AudioSource 的情形不在本次范围内）
- 音效资源的运行时加载 / 卸载（由调用方管理 AudioClip 引用）
- 音轨优先级排队系统

### Approach

基于 Unity AudioMixer 实现分层音轨。`AudioManager` 单例持有所有音轨实例，每条音轨封装一个 `AudioSource` + Mixer Group 引用，提供 Play / Pause / Stop 接口并支持淡入淡出协程。`SettingsPanel` 在 `BindControls()` / `SyncUIFromPrefs()` 的现有模式下扩展三个音量 Slider 的绑定逻辑，双向同步。

扩展新音轨只需要：① 在 Mixer Asset 中添加 Group，② 在 `AudioManager` Inspector 中添加一条 `AudioTrackConfig` 配置，零代码改动。
