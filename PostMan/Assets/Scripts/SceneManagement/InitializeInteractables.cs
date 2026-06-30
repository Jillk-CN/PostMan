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

        /*旧实现,现在改为调用InteractableManager
        [SerializeField]
        [Tooltip("这些可交互物体刚进入场景时可以交互吗")]
        private bool interactableOnInit;
        [SerializeField]
        [Tooltip("这些可交互物体刚进入场景时可以选中吗")]
        private bool selectableOnInit;
        [SerializeField]
        [Tooltip("玩家在哪个场景时,应用这个初始化")]
        private int targetSceneOrder;
        [SerializeField]
        [Tooltip("要初始化的可交互物体")]
        //假定一个物体上的可交互物体是同时启用同时禁用的,否则因作为另一个物体
        private GameObject[] interactableObjects;
        [SerializeField]
        [Tooltip("要初始化的可交互物体")]
        private GameObject[] selectableObjects;
         
         */
        private void OnEnable()
        {
            SceneInitializer.Instance.Register(Init); 
        }
        private void Init(int sceneOrder,string sceneName)
        {
            /*旧的实现
            if (targetSceneOrder!=sceneOrder)
            {
                return;
            }
            IInteractable[] interactables;
            for (int i = 0; i < this.interactableObjects.Length; i++)
            {
                interactables = this.interactableObjects[i].GetComponents<IInteractable>();
                foreach (var item in interactables)
                {
                    item.CanInteract = interactableOnInit;
                }
            }
            ISelectable[] selectables;
            for (int i = 0; i < selectableObjects.Length; i++)
            {
                selectables = selectableObjects[i].GetComponents<ISelectable>();
                foreach (var item in selectables)
                {
                    item.CanSelect = selectableOnInit;
                }
            } 
             */

            if (targetSceneOrder!=sceneOrder)
            {
                return;
            }
            InteractableManager.Instance.ApplyState(processListSO);
        }
    }
}
