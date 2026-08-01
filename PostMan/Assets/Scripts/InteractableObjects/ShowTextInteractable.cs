using PostMan.Scene;
using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class ShowTextInteractable : MonoBehaviour,IInteractable
    {
        [Tooltip("限制只在指定的场景次序下显示；设为 -1 则不限制（在任何场景都可交互）")]
        public int sceneOrder = -1;
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        [SerializeField]
        private ReadingContent content;
        public void InteractWith(PlayerInteractor player)
        {
            if(sceneOrder >= 0 && sceneOrder != SceneInitializer.Instance.SceneOrder)
            {
                Debug.LogWarning($"[ShowTextInteractable] sceneOrder({sceneOrder}) 与当前场景次序({SceneInitializer.Instance.SceneOrder})不匹配，已跳过");
                return;
            }

            if (content == null)
            {
                Debug.LogError("[ShowTextInteractable] content 未赋值，请在 Inspector 中指定 ReadingContent 资源");
                return;
            }

            TextPanel.Instance.ShowText(content);
        }
        public void SetContent(ReadingContent content)
        {
            this.content = content;
        }
    }
}