using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 挂载需要震动的物体上
    /// </summary>
    public class SinShakeEffect :MonoBehaviour
    {

        #region 水平方向的偏移
        public const int LEFT = -1; 
        public const int RIGHT = 1;
        private const float HROIZONTAL_OFFSET_MAX=180f;

        private int horizontalDirection;//水平方向上朝哪里偏移
        private bool enableHorizontalShake;
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

        #region 竖直方向的震动
        /// <summary>
        /// 带入sin的时间
        /// </summary>
        private float x;
        [Header("竖直方向的参数")]
        /// <summary>
        /// 频率
        /// </summary>
        [SerializeField]
        private float frequency;
        private float omega;
        /// <summary>
        /// 振幅
        /// </summary>
        public float amplitude;

        [Tooltip("从震动回到正常的复原时间,不要太长")]
        [SerializeField]
        private float backVerticalDuration;
        private float backVerticalElapsed;
        #endregion

        /// <summary>
        /// 当前是否在震动(水平/竖直)
        /// </summary>
        private bool shaking;

        private void Start()
        {
            omega = 2 * Mathf.PI * frequency;
            shaking = false;
            this.horizontalDirection = 0;
            enableHorizontalShake = false;
            horizontalElapsed = 0;
            backVerticalElapsed = 0;
            backHorizontalElapsed = 0;
        }
        private void Update()
        {
            //Debug.LogFormat("{0}", this.transform.localEulerAngles);
            if (!shaking)
            {
                BackVerticalTransition();
                BackHorizontalTransition();
                return;
            }
            //竖直方向的震动
            Vector3 shakePosition = this.transform.localPosition;
            //y=Asin(ωx)
            shakePosition.y = amplitude * Mathf.Sin(omega * x);
            this.transform.localPosition = shakePosition;
            x += Time.deltaTime;


            if (!enableHorizontalShake)
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
            shaking = true;
            horizontalElapsed = 0;
            x = 0;
        }
        public void StopShake()
        {
            shaking = false;
            backVerticalElapsed = 0;
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
                    enableHorizontalShake = false;
                    backHorizontalElapsed = 0;
                    horizontalElapsed = 0;
                }
            }
            else
            {
                enableHorizontalShake = true;
            }
            this.horizontalDirection = direction;
        }
        public void SetFrequency(float frequency)
        {
            this.frequency = frequency;
            omega = 2 * Mathf.PI * frequency;
        }

        /// <summary>
        /// 竖直方向上过渡回原位
        /// </summary>
        private void BackVerticalTransition()
        {
            if (backVerticalElapsed >= backVerticalDuration)
            {
                return;
            }
            backVerticalElapsed += Time.deltaTime;
            float rate = backVerticalElapsed / backVerticalDuration;
            Vector3 shakePosition = this.transform.localPosition;
            shakePosition.y =
            Mathf.Lerp(shakePosition.y, 0, rate); 
            this.transform.localPosition = shakePosition;

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
