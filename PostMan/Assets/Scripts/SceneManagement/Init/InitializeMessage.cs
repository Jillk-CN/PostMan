using PostMan.Player;
using PostMan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Scene
{
    public class InitializeMessage : MonoBehaviour
    {
        [Header("初始化参数,编辑器内手动配置")]
        [SerializeField]
        [Tooltip("玩家在哪个场景时,应用这个初始化")]
        private int targetSceneOrder;
        [SerializeField]
        private ShowTextInteractable textInteractable;//鉴于这个可交互物体只有一个,就不用数组了
        [SerializeField]
        private ReadingContent message;
        private void OnEnable()
        {
            SceneInitializer.Instance.Register(Init); 
        }
        private void Init(int sceneOrder,string sceneName)
        {
            if (targetSceneOrder!=sceneOrder)
            {
                return;
            }
            textInteractable.SetContent(message);
        }
    }
}
