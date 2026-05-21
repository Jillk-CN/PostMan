using PostMan.Common;
using PostMan.InputManagement;
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

        [Header("交互设置")]
        [Tooltip("交互的最大距离")]
        [SerializeField]
        private float interactDistance;
        [Tooltip("可交互物体的层级")]
        [SerializeField]
        private LayerMask detectLayer;

        private Transform sightPoint;

        private void Start()
        {
            sightPoint = this.transform.FindChildByName(nameof(sightPoint));
            input = GameInputManager.Instance.
                GetInputSystemSource<PlayerInteractInputSource>();
        }

        private void Update()
        {
            DetectInteractable();

            if (input != null && input.GetInteract())
            {
                //Debug.LogWarning("正在交互吗?"+interactInput.GetInteract());
                Interact();
            }
        }

        /// <summary>
        /// 与可交互物体交互
        /// </summary>
        public void Interact()
        {

        }
        /// <summary>
        /// 检测可交互物体
        /// </summary>
        private void DetectInteractable()
        {
            //检测物体
            bool detected = Physics.Raycast(sightPoint.transform.position, sightPoint.transform.forward, out RaycastHit hit, interactDistance, detectLayer);
            if (!detected)
            {
                //Deselect();
                return;
            }
            // 从命中物体向上查找 IInteractable（支持挂载在任意层级）
            IInteractable[] detectedInteractables =
                hit.collider.GetComponentsInParent<IInteractable>();
            //如果没找到,去根物体查找,出于某些原因才这样写
            if (detectedInteractables==null)
            {
                detectedInteractables = hit.collider.transform.root.
                    GetComponents<IInteractable>();
            }
            Transform newTarget = hit.collider.transform.root;
            bool hasInteractable = (detectedInteractables.Length != 0);

           // Debug.LogWarningFormat("target:{0}, {1},{2}",
           //     newTarget.name,hasInteractable,detectedInteractables?.Length);
           /*
            
            if (!hasInteractable)
            {
                Deselect();
                return;
            }
            // 检测到不同的可交互物体
            if (newTarget !=target)
            {
                //取消选中之前的可交互物体
                Deselect();

                target = newTarget;
                targetInteractables = detectedInteractables;
                Select();
            }
            */

        }

    }
}
