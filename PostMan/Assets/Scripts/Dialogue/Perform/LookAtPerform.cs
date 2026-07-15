using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class LookAtPerform : DialoguePerform
    {
        private LookDataProvider lookData;

        public override void Perform()
        {
            lookData.motion.LookAt(lookData.target.position, lookData.lookDuration);
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
             lookData = data as LookDataProvider;
        }

    }
}
