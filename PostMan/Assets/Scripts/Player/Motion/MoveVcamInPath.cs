using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    public class MoveVcamInPath : MonoBehaviour
    {
        [Header("要配置的相机参数")]
        [HideInInspector]
        [Tooltip("要移动的相机")]
        public CinemachineVirtualCamera vcam;
        [Tooltip("初始位置")]
        public float startPosition=0;
        [Tooltip("重点位置")]
        public float endPosition=1;

        [SerializeField]
        [Tooltip("移动持续时间")]
        public float moveDuration=3f;
        private float elapsed; 
        private bool moving;
        private Action completeCallback;

        //[Tooltip("路径")]
        private CinemachineTrackedDolly trackedDolly;
        
        private void Start()
        {
            vcam = this.GetComponent<CinemachineVirtualCamera>();
            trackedDolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
        }
        private void Update()
        {
            if (!moving)
            {
                return;
            }
            elapsed += Time.deltaTime;
            if (elapsed>moveDuration)
            {
                this.moving = false;
                completeCallback?.Invoke();
            }
            trackedDolly.m_PathPosition =
                Mathf.Lerp(startPosition, endPosition, elapsed / moveDuration);
            
        }
        public void MoveVcam(Action completed)
        {
            this.elapsed = 0;
            this.moving = true;
            this.completeCallback = completed;
            //trackedDolly.m_PathPosition        
        }
        public void StopMoving()
        {
            this.moving = false;
        }
    }
}
