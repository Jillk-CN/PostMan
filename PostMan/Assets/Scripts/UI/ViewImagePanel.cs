using PostMan.Common;
using PostMan.InputManagement;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PostMan.UI
{
    /// <summary>
    /// 这个项目没有UI框架,不能总依赖单例对吧,sigh~
    /// </summary>
    public class ViewImagePanel : MonoSingleton<ViewImagePanel>
    {
        private ImageViewer viewer;
        //非常不情愿地做成单例
        private Button quitButton;
        //输入来源
        private UIReturnInputSource input;
        private void Awake()
        {
            quitButton = this.transform.FindChildByName(nameof(quitButton)).
                GetComponent<Button>();
            quitButton.onClick.AddListener(Hide);
            viewer = this.transform.FindChildByName(nameof(viewer)).
                GetComponent<ImageViewer>();
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
        public void ShowImage(Sprite sprite)
        {
            viewer.SetImage(sprite);
            this.gameObject.SetActive(true);

            //没办法,由于没有好的框架,这里还要手动禁用Pause
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(false);
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(true);
            GameInputManager.Instance.SetPlayerAllInput(false);
            GameInputManager.Instance.ShowCursor();
        }
        /// <summary>
        /// 隐藏笔记内容
        /// </summary>
        private void Hide()
        {
            this.gameObject.SetActive(false);
            GameInputManager.Instance.SetInputSystemSource<PauseInputSource>(true);
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(false);
            GameInputManager.Instance.SetPlayerAllInput(true);
            GameInputManager.Instance.HideCursor();
        }
    }
}
