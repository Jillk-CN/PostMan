using PostMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 交互检测类,负责检测可被选中的物体
    /// </summary>
    public class PlayerDetector:MonoBehaviour
    {
        [Header("交互设置")]
        [Tooltip("交互的最大距离")]
        [SerializeField]
        private float detectDistance;
        [Tooltip("可交互物体的层级")]
        [SerializeField]
        private LayerMask detectLayer;

        private Transform sightPoint;
        private Transform detectedObject;
        private ISelectable[] detectedSelectables;
        private void Start()
        {
            sightPoint = this.transform.FindChildByName(nameof(sightPoint));
        }
        private void Update()
        {
            Detect();     
        }

        public Transform GetDetectedObject()
        {
            return this.detectedObject;
        }
        private void Detect()
        {
            //检测物体
            bool detected = Physics.Raycast(sightPoint.transform.position,
                sightPoint.transform.forward,
                out RaycastHit hit, detectDistance, detectLayer);
            if (!detected)
            {
                Deselect();
                return;
            }

            Transform newTarget = null;
            //只在射线检测到的物体或者根物体查找
            // 从命中物体向上查找 IInteractable（支持挂载在任意层级）
            ISelectable[] selectables =
                hit.collider.GetComponents<ISelectable>();
            //如果没找到,去根物体查找
            if (selectables.Length!=0)
            {
                newTarget = hit.collider.transform;
            }
            else
            {
                selectables = hit.collider.transform.root.
                    GetComponents<ISelectable>();
                if (selectables.Length==0)
                {
                    Deselect();
                    return;
                }
                newTarget = hit.collider.transform.root;
            }
            this.detectedObject = newTarget;
            this.detectedSelectables = selectables;
            Select();

        }
        private void Select()
        {
            foreach (var selectable in detectedSelectables)
            {
                if (!selectable.Selected)
                {
                    selectable.Select();
                }
            }
        }
        private void Deselect()
        {
            this.detectedObject = null;
            if (this.detectedSelectables==null)
            {
                return;
            }
            foreach (var selectable in detectedSelectables)
            {
                if (selectable.Selected)
                {
                    selectable.Deselect();
                }
            }
            this.detectedSelectables = null;
        }
    }
}
