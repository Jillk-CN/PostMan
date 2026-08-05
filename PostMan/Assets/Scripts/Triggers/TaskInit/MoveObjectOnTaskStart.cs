using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan
{
    public class MoveObjectOnTaskStart : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("要设置的物体")]
        private GameObject obj;
        [SerializeField]
        private Vector3 pos;
        [SerializeField]
        private Vector3 euler;

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
            this.obj.transform.position = pos;
            this.obj.transform.eulerAngles = euler;

                
        }


    }
}
