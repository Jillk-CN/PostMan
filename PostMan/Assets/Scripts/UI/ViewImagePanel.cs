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
        private void Awake()
        {
            quitButton = this.transform.FindChildByName(nameof(quitButton)).
                GetComponent<Button>();
            quitButton.onClick.AddListener(Hide);
            viewer = this.transform.FindChildByName(nameof(viewer)).
                GetComponent<ImageViewer>();
            this.gameObject.SetActive(false);
        }
        public void ShowImage(Sprite sprite)
        {
            viewer.SetImage(sprite);
            this.gameObject.SetActive(true);
            GameInputManager.Instance.SetInputSystemSource<UIReturnInputSource>(true);
            GameInputManager.Instance.SetInputSystemSource<PlayerMotionInputSource>(false);
            GameInputManager.Instance.SetInputSystemSource<PlayerSightInputSource>(false);
            GameInputManager.Instance.ShowCursor();
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
