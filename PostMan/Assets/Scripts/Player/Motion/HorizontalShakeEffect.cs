using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 水平方向的震动偏移,挂载需要震动的物体上
    /// </summary>
    public class HorizontalShakeEffect :MonoBehaviour
    {

        #region 水平方向的偏移
        public const int LEFT = -1; 
        public const int RIGHT = 1;
        private const float HROIZONTAL_OFFSET_MAX=180f;

        private int horizontalDirection;//水平方向上朝哪里偏移
        //private bool enableHorizontalShake;
        [Header("水平方向的参数")]
        [Tooltip("水平方向的最大偏移")]
        [SerializeField]
        [Range(0,180)]
        private float horizontalOffset;

        [Tooltip("水平方向偏移的过渡时间")]
        [SerializeField]
        private float horizontalDuration;
        private float horizontalElapsed;

        [Tooltip("水平方向从震动回到正常的复原时间,不要太长")]
        [SerializeField]
        private float backHorizontalDuration;
        private float backHorizontalElapsed;
        #endregion


        /// <summary>
        /// 当前是否在震动(水平/竖直)
        /// </summary>
        private bool shaking;
        private void Start()
        {
            shaking = false;
            this.horizontalDirection = 0;
            //enableHorizontalShake = false;
            horizontalElapsed = 0;
            backHorizontalElapsed = 0;
        }
        private void Update()
        {
            //Debug.LogFormat("{0}", this.transform.localEulerAngles);
            if (!shaking)
            {
                BackHorizontalTransition();
                return;
            }
            //水平方向的震动
            Vector3 horizontalEuler = this.transform.localEulerAngles;
            float offset =horizontalEuler.z;

            //由于unity里限制在[0,360],处理负数要特殊处理
            if (offset>HROIZONTAL_OFFSET_MAX)
            {
                offset -= 360;
            }
            offset =
                Mathf.Lerp(offset, horizontalOffset*horizontalDirection,
                horizontalElapsed / horizontalDuration);
            horizontalEuler.z = offset;

            //Debug.LogFormat("{0} to {1}", this.transform.localEulerAngles, horizontalEuler);

            this.transform.localEulerAngles = horizontalEuler;

            horizontalElapsed += Time.deltaTime;
            
        }

        public void StartShake()
        {
            if (shaking)
            {
                return;
            }
            shaking = true;
            horizontalElapsed = 0;
        }
        public void StopShake()
        {
            if (!shaking)
            {
                return;
            }
            shaking = false;
        }
        /// <summary>
        /// 设置水平震动方向
        /// </summary>
        /// <param name="direction">1是右边,-1是左边,0是无水平偏移</param>
        public void SetHorizontalDirection(int direction)
        {
            if (direction==0)
            {
                //防止重复设置
                if (this.horizontalDirection!=0)
                {
                    //enableHorizontalShake = false;
                    backHorizontalElapsed = 0;
                    horizontalElapsed = 0;
                }
            }
            else
            {
                //enableHorizontalShake = true;
            }
            this.horizontalDirection = direction;
        }
        /// <summary>
        /// 水平方向上过渡回原位
        /// </summary>
        private void BackHorizontalTransition()
        {
            //水平方向的震动复原
            Vector3 horizontalEuler = this.transform.localEulerAngles;
            float offset =horizontalEuler.z;
            //由于unity里限制在[0,360],处理负数要特殊处理
            if (offset>HROIZONTAL_OFFSET_MAX)
            {
                offset -= 360;
            }
            offset =
                Mathf.Lerp(offset, 0,
                backHorizontalElapsed / backHorizontalDuration);
            horizontalEuler.z = offset;
            this.transform.localEulerAngles = horizontalEuler;

            backHorizontalElapsed += Time.deltaTime;
        }
    }
}
