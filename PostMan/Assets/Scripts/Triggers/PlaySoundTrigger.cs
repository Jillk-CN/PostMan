using PostMan.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    public class PlaySoundTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool triggerOnce=true;
        [SerializeField]
        private bool canTrigger=true;

        [Header("音效参数设置")]
        [SerializeField]
        private AudioTrackId track;
        [SerializeField]
        private AudioClip clip;
        [SerializeField]
        private bool loop = false;
        [SerializeField]
        private bool fadeIn = false;
        [SerializeField]
        private float fadeInDuration = 0.5f;
        [SerializeField]
        private float volume = 1f;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && canTrigger)
            {
                AudioManager.Instance.Play
                    (track, clip, loop, fadeIn, fadeInDuration, volume);
                if (triggerOnce)
                {
                    this.canTrigger = false;
                }
            }
        } 
    }
}
