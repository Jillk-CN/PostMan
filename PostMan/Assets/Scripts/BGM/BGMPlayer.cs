using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioTrackId bgmTrackId = AudioTrackId.BGM;
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private bool loop = true;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private bool isFadeIn = true;
    [SerializeField] private bool isFadeOut = true;
    [SerializeField] private bool playOnAwake = false;

    private void Awake()
    {
        if (playOnAwake)
        {
            AudioManager.Instance.Play(bgmTrackId, bgmClip, loop, isFadeIn, fadeInDuration, volume);
        }
    }
}
