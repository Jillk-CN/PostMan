using System.Collections;
using System.Collections.Generic;
using PostMan.Common;
using UnityEngine;
using UnityEngine.Audio;

namespace PostMan.AudioSystem
{
    /// <summary>
    /// 集中式音频管理单例。
    /// 通过 Unity AudioMixer 实现分层音量控制（Master / BGM / SFX）。
    /// 音量设置持久化到 PlayerPrefs，游戏启动时自动恢复。
    /// </summary>
    public class AudioManager : MonoSingleton<AudioManager>
    {
        // ─────────────────────────────────────────────
        // PlayerPrefs 键名常量（供 SettingsPanel 读取）
        // ─────────────────────────────────────────────

        /// <summary>主音量 PlayerPrefs 键名</summary>
        public const string KeyMasterVolume = "MasterVolume";

        /// <summary>背景音乐音量 PlayerPrefs 键名</summary>
        public const string KeyBGMVolume = "BGMVolume";

        /// <summary>音效音量 PlayerPrefs 键名</summary>
        public const string KeySFXVolume = "SFXVolume";

        // ─────────────────────────────────────────────
        // AudioMixer
        // ─────────────────────────────────────────────

        [Header("AudioMixer")]
        [Tooltip("项目主 AudioMixer Asset（含 Master/BGM/SFX 三组 exposed 参数）")]
        [SerializeField] private AudioMixer mainMixer;

        /// <summary>AudioMixer exposed 参数名：主音量</summary>
        private const string MixerParamMaster = "MasterVolume";

        /// <summary>AudioMixer exposed 参数名：BGM 音量</summary>
        private const string MixerParamBGM = "BGMVolume";

        /// <summary>AudioMixer exposed 参数名：SFX 音量</summary>
        private const string MixerParamSFX = "SFXVolume";

        // ─────────────────────────────────────────────
        // 音轨配置
        // ─────────────────────────────────────────────

        [Header("音轨配置")]
        [Tooltip("所有音轨的配置列表，每条对应一个 AudioTrackId 枚举值")]
        [SerializeField] private List<AudioTrackConfig> tracks = new List<AudioTrackConfig>();

        /// <summary>运行时快速查找表（Init() 时构建）</summary>
        private Dictionary<AudioTrackId, AudioTrackConfig> _trackMap;

        /// <summary>每条音轨当前正在执行的淡变协程引用，防止竞态</summary>
        private readonly Dictionary<AudioTrackId, Coroutine> _fadeCoroutines
            = new Dictionary<AudioTrackId, Coroutine>();

        // ─────────────────────────────────────────────
        // 初始化
        // ─────────────────────────────────────────────

        /// <summary>
        /// 单例首次访问时调用（MonoSingleton 模板方法）。
        /// 构建音轨字典并从 PlayerPrefs 恢复上次的音量设置。
        /// </summary>
        protected override void Init()
        {
            BuildTrackMap();
            RestoreVolumeFromPrefs();
        }

        /// <summary>将 tracks 列表转换为 Dictionary，方便 O(1) 查找。</summary>
        private void BuildTrackMap()
        {
            _trackMap = new Dictionary<AudioTrackId, AudioTrackConfig>(tracks.Count);
            foreach (var config in tracks)
            {
                if (config == null) continue;
                if (_trackMap.ContainsKey(config.trackId))
                {
                    Debug.LogWarning($"[AudioManager] 重复的 AudioTrackId: {config.trackId}，后者将被忽略。");
                    continue;
                }
                // 将 AudioSource 的 output 绑定到对应的 MixerGroup
                if (config.audioSource != null && config.mixerGroup != null)
                    config.audioSource.outputAudioMixerGroup = config.mixerGroup;

                _trackMap[config.trackId] = config;
            }
        }

        /// <summary>从 PlayerPrefs 读取并应用保存的音量值。</summary>
        private void RestoreVolumeFromPrefs()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(KeyMasterVolume, 1f));
            SetBGMVolume(PlayerPrefs.GetFloat(KeyBGMVolume, 1f));
            SetSFXVolume(PlayerPrefs.GetFloat(KeySFXVolume, 1f));
        }

        // ─────────────────────────────────────────────
        // 公开 API：播放控制
        // ─────────────────────────────────────────────

        /// <summary>
        /// 在指定音轨上播放一个 AudioClip。
        /// 若该音轨正在播放，先停止再启动新的播放。
        /// </summary>
        /// <param name="trackId">目标音轨 ID</param>
        /// <param name="clip">要播放的 AudioClip</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="fadeIn">是否淡入</param>
        /// <param name="fadeInDuration">淡入时长（秒）</param>
        /// <param name="volume">音轨内局部音量（0~1，最终受 MixerGroup 调节）</param>
        public void Play(AudioTrackId trackId, AudioClip clip,
                         bool loop = false,
                         bool fadeIn = false, float fadeInDuration = 0.5f,
                         float volume = 1f)
        {
            if(clip == null)
            {
                Debug.LogError("音频为空");
                return;
            }

            if (!TryGetSource(trackId, out AudioSource source)) return;

            StopFadeCoroutine(trackId);

            source.clip   = clip;
            source.loop   = loop;
            source.volume = fadeIn ? 0f : volume;
            source.Play();

            if (fadeIn)
                _fadeCoroutines[trackId] = StartCoroutine(FadeVolume(source, 0f, volume, fadeInDuration));
        }

        /// <summary>
        /// 暂停指定音轨的播放。
        /// </summary>
        /// <param name="trackId">目标音轨 ID</param>
        /// <param name="fadeOut">是否先淡出再暂停</param>
        /// <param name="fadeOutDuration">淡出时长（秒）</param>
        public void Pause(AudioTrackId trackId,
                          bool fadeOut = false, float fadeOutDuration = 0.5f)
        {
            if (!TryGetSource(trackId, out AudioSource source)) return;

            StopFadeCoroutine(trackId);

            if (fadeOut)
                _fadeCoroutines[trackId] = StartCoroutine(
                    FadeVolume(source, source.volume, 0f, fadeOutDuration, source.Pause));
            else
                source.Pause();
        }

        /// <summary>
        /// 停止指定音轨的播放。
        /// </summary>
        /// <param name="trackId">目标音轨 ID</param>
        /// <param name="fadeOut">是否先淡出再停止</param>
        /// <param name="fadeOutDuration">淡出时长（秒）</param>
        public void Stop(AudioTrackId trackId,
                         bool fadeOut = false, float fadeOutDuration = 0.5f)
        {
            if (!TryGetSource(trackId, out AudioSource source)) return;

            StopFadeCoroutine(trackId);

            if (fadeOut)
                _fadeCoroutines[trackId] = StartCoroutine(
                    FadeVolume(source, source.volume, 0f, fadeOutDuration, source.Stop));
            else
                source.Stop();
        }

        /// <summary>
        /// 获取指定音轨当前加载的 AudioClip。
        /// 若音轨未配置或 AudioSource 为空则返回 null。
        /// </summary>
        /// <param name="trackId">目标音轨 ID</param>
        /// <returns>当前 AudioClip；音轨未播放时也可能为 null</returns>
        public AudioClip GetClip(AudioTrackId trackId)
        {
            if (!TryGetSource(trackId, out AudioSource source)) return null;
            return source.clip;
        }

        // ─────────────────────────────────────────────
        // 公开 API：音量控制
        // ─────────────────────────────────────────────

        /// <summary>
        /// 设置主音量（线性值 0~1）并持久化到 PlayerPrefs。
        /// </summary>
        public void SetMasterVolume(float linearValue)
        {
            ApplyMixerVolume(MixerParamMaster, linearValue);
            PlayerPrefs.SetFloat(KeyMasterVolume, linearValue);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置背景音乐音量（线性值 0~1）并持久化到 PlayerPrefs。
        /// </summary>
        public void SetBGMVolume(float linearValue)
        {
            ApplyMixerVolume(MixerParamBGM, linearValue);
            PlayerPrefs.SetFloat(KeyBGMVolume, linearValue);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 设置音效音量（线性值 0~1）并持久化到 PlayerPrefs。
        /// </summary>
        public void SetSFXVolume(float linearValue)
        {
            ApplyMixerVolume(MixerParamSFX, linearValue);
            PlayerPrefs.SetFloat(KeySFXVolume, linearValue);
            PlayerPrefs.Save();
        }

        // ─────────────────────────────────────────────
        // 内部辅助
        // ─────────────────────────────────────────────

        /// <summary>
        /// 将线性音量（0~1）转换为 dB 并推送到 AudioMixer 的 exposed 参数。
        /// 线性值 0 映射到 -80 dB（静音），1 映射到 0 dB。
        /// </summary>
        private void ApplyMixerVolume(string paramName, float linearValue)
        {
            if (mainMixer == null)
            {
                Debug.LogWarning($"[AudioManager] mainMixer 未赋值，无法设置 {paramName}。");
                return;
            }
            // 避免 Log10(0) = -∞，将最小值限制为 0.0001
            float db = Mathf.Log10(Mathf.Max(linearValue, 0.0001f)) * 20f;
            mainMixer.SetFloat(paramName, db);
        }

        /// <summary>尝试从字典中取得指定音轨的 AudioSource，失败时输出警告。</summary>
        private bool TryGetSource(AudioTrackId trackId, out AudioSource source)
        {
            source = null;
            if (_trackMap == null)
            {
                Debug.LogWarning("[AudioManager] _trackMap 未初始化，请确认 Init() 已执行。");
                return false;
            }
            if (!_trackMap.TryGetValue(trackId, out AudioTrackConfig config) || config?.audioSource == null)
            {
                Debug.LogWarning($"[AudioManager] 未找到音轨 {trackId} 的有效 AudioSource 配置。");
                return false;
            }
            source = config.audioSource;
            return true;
        }

        /// <summary>停止指定音轨正在运行的淡变协程（若存在）。</summary>
        private void StopFadeCoroutine(AudioTrackId trackId)
        {
            if (_fadeCoroutines.TryGetValue(trackId, out Coroutine coroutine) && coroutine != null)
            {
                StopCoroutine(coroutine);
                _fadeCoroutines[trackId] = null;
            }
        }

        /// <summary>
        /// 在指定时长内将 AudioSource 音量从 from 线性过渡到 to。
        /// 过渡完成后可选执行一个回调（如 Stop / Pause）。
        /// </summary>
        private IEnumerator FadeVolume(AudioSource source,
                                       float from, float to, float duration,
                                       System.Action onComplete = null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed     += Time.deltaTime;
                source.volume = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            source.volume = to;
            onComplete?.Invoke();
        }
    }
}
