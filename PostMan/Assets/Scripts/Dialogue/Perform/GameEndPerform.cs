using PostMan.InputManagement;
using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    /// <summary>
    /// 屎山代码
    /// </summary>
    public class GameEndPerform :DialoguePerform 
    {
        public GameEndDataProvider data;
        public override void Perform()
        {
            BlackScreen.Instance.BlackIn("The End",0f, () => 
            { data.StartCoroutine(DelaySwitch()); });
        }
        private IEnumerator DelaySwitch()
        {
            yield return new WaitForSeconds(1f);
            GameSceneManager.Instance.SwitchScenes(data.scenesToLoad, data.scenesToUnload);
            BlackScreen.Instance.BlackOut("", 1f);
            TitleUIManager.Instance?.ShowTitleUI(); // 恢复标题 UI
            GameInputManager.Instance.ShowCursor();
        }
        public override void ReceiveData(IPerformDataProvider data)
        {
            this.data = data as GameEndDataProvider;
        }


    }
}
