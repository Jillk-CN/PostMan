using PostMan.InputManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    public class ControlInputPerform : DialoguePerform
    {
        private bool enableInput;
        public override void Perform()
        {

            //禁用输入,注意对话时不允许打开设置面板,会有bug,要修有点麻烦
            GameInputManager.Instance.SetPlayerAllInput(enableInput);
            Debug.Log(enableInput);
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(enableInput);
        }

        public override void ReceiveData(IPerformDataProvider data)
        {
            enableInput = (data as ControlInputDataProvider).enableInput;
        }

    }
}
