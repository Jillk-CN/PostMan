# 任务：离开触发区域时恢复原 BGM

## 实现

- [x] **在 `AudioManager` 中新增 `GetClip` 方法**
  - 文件：`Assets/Scripts/AudioSystem/AudioManager.cs`
  - 在「公开 API：播放控制」区块的 `Stop` 方法之后插入新方法
  - 方法签名：`public AudioClip GetClip(AudioTrackId trackId)`
  - 调用 `TryGetSource(trackId, out AudioSource source)`，失败返回 `null`
  - 成功则返回 `source.clip`
  - 添加 XML `/// <summary>` 注释（中文）

- [x] **修改 `ColliderBGMSwitcher`：新增快照字段**
  - 文件：`Assets/Scripts/BGM/ColliderBGMSwitcher.cs`
  - 在「运行时状态」区块的 `_triggered` 字段之后新增：
    - `private AudioClip _previousClip;`（中文 `/// <summary>`）
    - `private float _previousVolume = 1f;`（中文 `/// <summary>`）

- [x] **修改 `ColliderBGMSwitcher`：OnTriggerEnter 增加快照**
  - 在调用 `AudioManager.Instance.Stop` 之前插入两行：
    ```csharp
    _previousClip   = AudioManager.Instance.GetClip(AudioTrackId.BGM);
    _previousVolume = 1f;
    ```

- [x] **修改 `ColliderBGMSwitcher`：新增 `OnTriggerExit` 方法**
  - 在 `OnTriggerEnter` 方法之后新增 `private void OnTriggerExit(Collider other)`
  - 添加 XML `/// <summary>` 注释（中文）
  - Tag 过滤：`triggerTag` 非空且 `!other.CompareTag(triggerTag)` → `return`
  - 未触发保护：`!_triggered` → `return`（从未成功进入过则无需还原）
  - 调用 `AudioManager.Instance.Stop(AudioTrackId.BGM, stopFadeOut, stopFadeOutDuration)`
  - 若 `_previousClip != null`，调用 `AudioManager.Instance.Play(AudioTrackId.BGM, _previousClip, loop, playFadeIn, playFadeInDuration, _previousVolume)`

## Unity Editor 验证

- [ ] 打开 Unity Editor，确认控制台无编译错误
- [ ] 在场景中配置一个带 `ColliderBGMSwitcher` 的触发区域，赋值 `newClip`
- [ ] 进入区域 → 确认 BGM 切换到 `newClip`
- [ ] 离开区域 → 确认 BGM 还原到进入前的曲目（若进入前无 BGM 则静音）
- [ ] `triggerOnce = true` 时：再次进入不切换，再次离开不还原
- [ ] `_previousClip == null` 场景（进入前无 BGM）：离开时只停止，不播放

## 验证清单

- [ ] `GetClip` 在音轨未配置时返回 `null` 而非抛出异常
- [ ] `OnTriggerExit` 的 Tag 过滤与 `OnTriggerEnter` 行为一致
- [ ] 快照发生在 `Stop` 之前（记录的是切换前的曲目）
- [ ] `_previousClip == null` 时 `OnTriggerExit` 只 Stop，不 Play
- [ ] 现有的 `OnTriggerEnter` 行为未受影响
