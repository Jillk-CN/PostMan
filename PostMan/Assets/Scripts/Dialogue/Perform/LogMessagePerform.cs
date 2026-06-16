using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class LogMessagePerform : DialoguePerform
    {
        private string msg;
        public override void Perform()
        {
            Debug.Log(msg);
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            msg = (data as LogMessageDataProvider).debugMessage;
        }
    }
}
