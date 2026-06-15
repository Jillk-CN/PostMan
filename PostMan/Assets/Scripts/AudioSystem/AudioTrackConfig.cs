using System;
using UnityEngine;
using UnityEngine.Audio;

namespace PostMan.AudioSystem
{
    /// <summary>
    /// 单条音轨的 Inspector 配置项。
    /// 在 AudioManager.tracks 列表中逐条填写，Init() 时构建成字典供运行时快速查找。
    /// </summary>
    [Serializable]
    public class AudioTrackConfig
    {
        [Tooltip("本条配置对应的音轨 ID，必须与 AudioTrackId 枚举值唯一对应")]
        public AudioTrackId trackId;

        [Tooltip("绑定的 AudioMixer Output Group（BGM / SFX 等）")]
        public AudioMixerGroup mixerGroup;

        [Tooltip("专属 AudioSource 组件，挂载在 AudioManager 子物体上")]
        public AudioSource audioSource;
    }
}
