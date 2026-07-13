using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Scene
{
    /// <summary>
    /// 初始化场景的可交互脚本
    /// </summary>
    public class InitializeInteractables : MonoBehaviour
    {
        [Header("初始化参数,编辑器内手动配置")]
        [SerializeField]
        private InteractablesProcessListSO processListSO;
        [SerializeField]
        [Tooltip("玩家在哪个场景时,应用这个初始化")]
        private int targetSceneOrder;

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
            InteractableManager.Instance.ApplyState(processListSO);
            Debug.LogWarning(sceneOrder);
        }
    }
}
