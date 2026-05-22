using PostMan.Common;
using PostMan.InputManagement;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 玩家交互类,负责检测可交互物体并提供交互方法
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        private PlayerInteractInputSource input;

        [Tooltip("交互的最大距离")]
        [SerializeField]
        private float interactDistance;
        private PlayerDetector detector;

        private void Start()
        {
            detector = this.GetComponent<PlayerDetector>();
            input = GameInputManager.Instance.
                GetInputSystemSource<PlayerInteractInputSource>();
        }

        private void Update()
        {
            if (input.GetInteract())
            {
                Interact();
            }
        }

        /// <summary>
        /// 与可交互物体交互
        /// </summary>
        private void Interact()
        {
            IInteractable[] interactables = FindInteractables();
            if (interactables==null)
            {
                return;
            }
            foreach (var interactable in interactables)
            {
                interactable.InteractWith(this);
            }
        }
        /// <summary>
        /// 找到可交互的脚本,按优先级降序排序
        /// </summary>
        /// <returns></returns>
        private IInteractable[] FindInteractables()
        {
            Transform detectedObject = this.detector.GetDetectedObject();
            IInteractable[] interactables =
                detectedObject.GetComponents<IInteractable>();
            if (interactables.Length==0)
            {
                return null;
            }

            interactables = interactables.
                Where((arg) => arg.CanInteract).
                OrderByDescending((arg) => arg.Priority).ToArray();
            return interactables;
        }
            
    }
}
