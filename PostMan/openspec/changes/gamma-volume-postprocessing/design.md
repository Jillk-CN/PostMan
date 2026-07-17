# Design: Gamma 值接入 URP Volume Post-Processing

## 架构总览

```
新增文件：
Assets/Scripts/VHSFilter/GammaVolumeController.cs   — MonoSingleton，管理 ColorAdjustments.postExposure

修改文件：
Assets/Scripts/UI/Title/SettingsPanel.cs             — OnGammaChanged() 和 SyncUIFromPrefs() 各加一行调用

Editor 手动操作：
Assets/Settings/SampleSceneProfile.asset            — 添加 ColorAdjustments Volume 组件并勾选 postExposure override
```

## 参数映射关系

Slider 值域：0.5（暗）→ 1.0（默认）→ 2.0（亮）

```
postExposure (EV) = (sliderValue - 1.0f) * 2.0f
```

| Slider | postExposure |
|--------|-------------|
| 0.5    | −1.0 EV     |
| 1.0    |  0.0 EV（默认，无变化） |
| 2.0    | +2.0 EV     |

选择 `postExposure` 而非 `LiftGammaGain.Gamma`，原因：
- `postExposure` 是全局 EV 偏移，语义与"Gamma 亮度"在玩家直觉上一致；
- 只需一个浮点参数，实现最简，不引入色彩分级复杂度；
- URP 2022.3 内置，无需额外 package。

## GammaVolumeController 设计

```csharp
// 继承项目现有 MonoSingleton 模式（与 VHSFilterController、AudioManager 一致）
public sealed class GammaVolumeController : MonoSingleton<GammaVolumeController>
{
    [SerializeField] private Volume _targetVolume;  // 引用场景 Volume 对象

    private ColorAdjustments _colorAdj;
    private const float DefaultGamma = 1f;

    protected override void Init()
    {
        // 创建运行时副本（与 VHSVolumeController 同一惯例）
        _targetVolume.profile = Instantiate(_targetVolume.profile);
        // 取出或动态添加 ColorAdjustments
        if (!_targetVolume.profile.TryGet(out _colorAdj))
            _colorAdj = _targetVolume.profile.Add<ColorAdjustments>(overrides: false);
        // 确保 postExposure 被覆盖
        _colorAdj.postExposure.overrideState = true;
        // 启动时从 PlayerPrefs 恢复
        Apply(PlayerPrefs.GetFloat("Gamma", DefaultGamma));
    }

    public void Apply(float sliderValue)
    {
        if (_colorAdj == null) return;
        _colorAdj.postExposure.value = (sliderValue - 1f) * 2f;
    }
}
```

## SettingsPanel 修改点（最小化）

只修改两处，不触碰其他方法：

```csharp
// OnGammaChanged()：在 PlayerPrefs.Save() 之后追加一行
GammaVolumeController.Instance?.Apply(value);

// SyncUIFromPrefs()：在 sliderGamma.SetValueWithoutNotify(...) 之后追加一行
GammaVolumeController.Instance?.Apply(PlayerPrefs.GetFloat("Gamma", 1f));
```

## 数据流

```
SettingsPanel.sliderGamma.onValueChanged(float value)
  → PlayerPrefs.SetFloat("Gamma", value) + Save()
  → GammaVolumeController.Instance.Apply(value)
  → _colorAdj.postExposure.value = (value - 1f) * 2f

游戏启动 / GammaVolumeController.Init()
  → Apply(PlayerPrefs.GetFloat("Gamma", 1f))   ← 恢复上次设置

SettingsPanel.OnEnable → SyncUIFromPrefs()
  → sliderGamma.SetValueWithoutNotify(saved)
  → GammaVolumeController.Instance.Apply(saved) ← 面板打开时渲染与 UI 同步
```

## 关键设计决策

**为什么不放进 VHSVolumeController？**
`VHSVolumeController` 的 `Apply(in VHSSettings)` 接口只在 VHS 开关变化时调用，而 Gamma 是独立的画面设置，与 VHS 开关无关，生命周期不同。拆为独立类职责更清晰，也与项目现有单例模式一致。

**为什么对 SampleSceneProfile 用运行时副本？**
与 `VHSVolumeController` 中 `Instantiate(_targetVolume.profile)` 的惯例一致，避免在 Play Mode 中修改持久化 Asset，防止 Editor 数据污染。

**标题场景 vs 游戏场景**
`GammaVolumeController` 挂载在 Persistent 场景（与 `VHSFilterController` 同一 GameObject 位置），两个场景共享同一实例，无需重复挂载。
