using PostMan.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class PlaySoundPerform :DialoguePerform 
    {
        private SoundDataProvider soundData;

        public override void Perform()
        {
            AudioManager.Instance.Play
                (soundData.trackId, soundData.sound,soundData.loop);
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
             soundData = data as SoundDataProvider;
        }

        
    }
}
