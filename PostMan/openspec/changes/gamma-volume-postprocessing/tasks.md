# Tasks: Gamma 值接入 URP Volume Post-Processing

## 实现任务

### Phase 1 — 新建 GammaVolumeController 脚本

- [ ] 创建 `Assets/Scripts/VHSFilter/GammaVolumeController.cs`：
  - 继承 `MonoSingleton<GammaVolumeController>`（与 `VHSFilterController` 保持一致）
  - `[SerializeField] private Volume _targetVolume` — Inspector 引用场景 Volume
  - `protected override void Init()`：
    1. `_targetVolume.profile = Instantiate(_targetVolume.profile)` — 创建运行时副本
    2. `_targetVolume.profile.TryGet(out _colorAdj)`，若为 null 则 `Add<ColorAdjustments>(overrides: false)`
    3. `_colorAdj.postExposure.overrideState = true`
    4. 调用 `Apply(PlayerPrefs.GetFloat("Gamma", 1f))` 恢复上次设置
  - `public void Apply(float sliderValue)`：`_colorAdj.postExposure.value = (sliderValue - 1f) * 2f`
  - 所有字段附 `[Tooltip]`，类和方法附 XML 注释（中文）

### Phase 2 — 修改 SettingsPanel

- [ ] 修改 `Assets/Scripts/UI/Title/SettingsPanel.cs`：
  - `OnGammaChanged(float value)`：在 `PlayerPrefs.Save()` 之后追加 `GammaVolumeController.Instance?.Apply(value);`，删除 `// TODO` 注释
  - `SyncUIFromPrefs()`：在 `sliderGamma.SetValueWithoutNotify(...)` 之后追加 `GammaVolumeController.Instance?.Apply(PlayerPrefs.GetFloat("Gamma", 1f));`

### Phase 3 — Editor 手动配置

- [ ] 在 Unity Editor 中打开 `Assets/Settings/SampleSceneProfile`，添加 `Color Adjustments` Volume Override，勾选 `Post Exposure` 的覆盖开关（override checkbox），值保持 0（对应 Slider = 1.0 默认值）
- [ ] 在 Persistent 场景中找到挂载 `VHSFilterController` 的 GameObject（或同级位置），添加 `GammaVolumeController` 组件，将场景 Volume 对象拖入 `_targetVolume` 字段

## 验证清单

- [ ] 进入游戏，打开设置面板，拖动 Gamma 滑条从 0.5 到 2.0，确认画面亮度实时变化
- [ ] 将 Gamma 滑条拖到非默认值（如 1.5），关闭设置面板后再打开，确认滑条显示值与画面亮度一致
- [ ] 关闭游戏重新启动，确认上次设置的 Gamma 值被正确恢复（画面亮度与保存值一致，不需要打开设置面板）
- [ ] 将 Gamma 滑条置于默认值 1.0，确认画面与未添加后处理时视觉一致（postExposure = 0 EV）
- [ ] 在 Editor Play Mode 中确认 `SampleSceneProfile` Asset 文件未被修改（运行时副本机制生效）
