using PostMan.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class SoundDataProvider : MonoBehaviour,IPerformDataProvider
    {
        [SerializeField]
        private int priority;
        [SerializeField]
        private int dialogueIndex;
        public int TargetDialogueIndex => dialogueIndex;
        public int Priority => priority;
        public string PerformName => nameof(PlaySoundPerform);

       public  AudioClip sound;
       public AudioTrackId trackId;
       public bool loop = false;
       public bool fadeIn = false;
       public float fadeInDuration = 0.5f;
       public float volume = 1f;
    }
}
