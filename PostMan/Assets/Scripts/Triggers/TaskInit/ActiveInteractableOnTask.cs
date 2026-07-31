using JetBrains.Annotations;
using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PostMan 
{
    public class ActiveInteractableOnTask : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("这些可交互物体刚进入场景时可以交互吗")]
        private bool interactableOnInit;
        [SerializeField]
        [Tooltip("这些可交互物体刚进入场景时可以选中吗")]
        private bool selectableOnInit;
        [SerializeField]
        [Tooltip("要设置的可交互物体")]
        //假定一个物体上的可交互物体是同时启用同时禁用的,否则因作为另一个物体
        private GameObject[] interactableObjects;
        [SerializeField]
        [Tooltip("要设置的可交互物体")]
        private GameObject[] selectableObjects;

        [SerializeField]
        private int taskIndex; 
        private void OnEnable()
        {
            TaskEventBus.OnTaskStarted += SetInteractable;
        }
        private void OnDisable()
        {
            TaskEventBus.OnTaskStarted -= SetInteractable;
        }

        private void SetInteractable(TaskRuntimeData obj)
        {
            if (taskIndex!=obj.Definition.taskIndex)
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
