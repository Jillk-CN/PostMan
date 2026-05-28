using PostMan.Common;
using PostMan.InputManagement;
using PostMan.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.UI
{
    /// <summary>
    /// 笔记UI,负责显示笔记内容
    /// </summary>
    public class NotePanel : MonoSingleton<NotePanel>
    {
        //非常不情愿地做成单例
        private Button quitButton;
        private TextMeshProUGUI titleText;
        private TextMeshProUGUI noteContent;
        private void Awake()
        {
            titleText = this.transform.FindChildByName(nameof(titleText)).
                GetComponent<TextMeshProUGUI>();
            noteContent = this.transform.FindChildByName(nameof(noteContent)).
                GetComponent<TextMeshProUGUI>();
            quitButton = this.transform.FindChildByName(nameof(quitButton)).
                GetComponent<Button>();
            quitButton.onClick.AddListener(Hide);
            this.gameObject.SetActive(false);
        }
        private void Start()
        {
        }


        public void ShowNote(string content)
        {
            //finishCallback = finishReading;
            if (content==null)
            {
                return;
            }
            this.gameObject.SetActive(true);
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(true);
            GameInputManager.Instance.SetInputSystemSource<PlayerMotionInputSource>(false);
            GameInputManager.Instance.SetInputSystemSource<PlayerSightInputSource>(false);
            GameInputManager.Instance.ShowCursor();

            /*
            this.gameObject.SetActive(true);

            this.titleText.text = content.GetTitle();
            this.noteContent.text = content.GetText();
             */
                    
        }
        /// <summary>
        /// 隐藏笔记内容
        /// </summary>
        private void Hide()
        {
            this.gameObject.SetActive(false);
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(false);
            GameInputManager.Instance.SetInputSystemSource<PlayerMotionInputSource>(true);
            GameInputManager.Instance.SetInputSystemSource<PlayerSightInputSource>(true);
            GameInputManager.Instance.HideCursor();
        }
    }
}
