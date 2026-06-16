using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class ShowSubtitlePerform : DialoguePerform
    {
        private string subtitleKey;
        public override void Perform()
        {
            SubtitleUI.Instance.TypeSubtitle(this.subtitleKey);
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            subtitleKey = (data as SubtitleDataProvider).subtitleKey;
        }
    }
}
