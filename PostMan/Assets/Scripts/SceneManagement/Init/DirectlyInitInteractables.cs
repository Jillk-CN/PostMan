using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Scene
{
    /// <summary>
    /// 出于某些原因,在street场景会用这个脚本来初始化
    /// </summary>
    public class DirectlyInitInteractables : MonoBehaviour
    {
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
        }
         




    }
}
