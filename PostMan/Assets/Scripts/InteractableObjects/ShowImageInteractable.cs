using PostMan.Scene;
using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class ShowImageInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Sprite image;
        [Tooltip("限制只在指定的场景次序下显示；设为 -1 则不限制（在任何场景都可交互）")]
        [SerializeField]
        private int sceneOrder = -1;
        [SerializeField]
        private bool canInteract = true;
        public bool CanInteract { get => this.canInteract; set => this.canInteract = value; }
        [SerializeField]
        private int priority;
        public int Priority { get => priority; set => priority=value; }

        public bool showQuitButton = true;
        public void InteractWith(PlayerInteractor player)
        {
            if(sceneOrder >= 0 && sceneOrder != SceneInitializer.Instance.SceneOrder)
            {
                Debug.LogWarning($"[ShowImageInteractable] sceneOrder({sceneOrder}) 与当前场景次序({SceneInitializer.Instance.SceneOrder})不匹配，已跳过");
                return;
            }

            if (image==null)
            {
                Debug.LogError("[ShowImageInteractable] image 未赋值，请在 Inspector 中指定 Sprite 资源");
                return;
            }
            ViewImagePanel.Instance.ShowImage(image,showQuitButton);            
        }
    }
}