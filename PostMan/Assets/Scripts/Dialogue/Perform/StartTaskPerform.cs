using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class StartTaskPerform : DialoguePerform
    {
        private DialogueTaskTrigger trigger;
        public override void Perform()
        {
            trigger?.TriggerTask();
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            trigger = (data as TaskTriggerDataProvider).trigger;
        }
    }
}
