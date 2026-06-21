using PostMan.AudioSystem;
using PostMan.Common;
using PostMan.InputManagement;
using PostMan.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PostMan.UI
{
    /// <summary>
    /// 单纯显示文字的UI,
    /// </summary>
    public class TextPanel : MonoSingleton<TextPanel>
    {
        //非常不情愿地做成单例
        private Button quitButton;
        //private TextMeshProUGUI titleText;
        private TextMeshProUGUI noteContent;
        //输入来源
        private UIReturnInputSource input;
        public AudioClip clickSound;
        protected override void Init()
        {
            base.Init();
           // titleText = this.transform.FindChildByName(nameof(titleText)).
           //     GetComponent<TextMeshProUGUI>();
            noteContent = this.transform.FindChildByName(nameof(noteContent)).
                GetComponent<TextMeshProUGUI>();
            quitButton = this.transform.FindChildByName(nameof(quitButton)).
                GetComponent<Button>();
            quitButton.onClick.AddListener(Hide);
            this.gameObject.SetActive(false);
            input = GameInputManager.Instance.GetInputSystemSource<UIReturnInputSource>();
        }
        private void Update()
        {
            if (input.GetReturn())
            {
                Hide();
            } 
        }

        public void ShowText(ReadingContent content)
        {
            //finishCallback = finishReading;
            if (content==null)
            {
                return;
            }
            this.gameObject.SetActive(true);
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(false);
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(true);
            GameInputManager.Instance.SetPlayerAllInput(false);
            GameInputManager.Instance.ShowCursor();

            //this.titleText.text = content.GetTitle();
            this.noteContent.text = content.GetText();
        }
        /// <summary>
        /// 隐藏笔记内容
        /// </summary>
        private void Hide()
        {
            if (clickSound != null) 
            {
                AudioManager.Instance.Play(AudioTrackId.FX , clickSound);
            }

            this.gameObject.SetActive(false);
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(true);
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(false);
            GameInputManager.Instance.SetPlayerAllInput(true);
            GameInputManager.Instance.HideCursor();
            
            
        }
    }
}
