using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan
{
    public class ActiveObjectOnTaskStart : MonoBehaviour
    {
        [SerializeField]
        private bool active;
        [SerializeField]
        [Tooltip("要设置的物体")]
        private GameObject[] objects;

        [SerializeField]
        private int taskIndex; 
        private void OnEnable()
        {
            TaskEventBus.OnTaskStarted += SetObject;
        }
        private void OnDisable()
        {
            TaskEventBus.OnTaskStarted -= SetObject;
        }

        private void SetObject(TaskRuntimeData obj)
        {
            if (taskIndex!=obj.Definition.taskIndex)
            {
                return;
            }
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(active);
            }

        }

    }
}
