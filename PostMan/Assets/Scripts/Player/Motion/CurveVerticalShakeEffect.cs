using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 自定义曲线效果的震动,挂载需要震动的物体上
    /// </summary>
    public class CurveVerticalShakeEffect : MonoBehaviour
    {
        [Header("震动曲线")]
        [SerializeField]
        private AnimationCurve walkCurveVertical;
        [SerializeField]
        private AnimationCurve runCurveVertical;

        [SerializeField]
        [Tooltip("震动归零的过渡总时间")]
        private float backDuration=1f;
        private float backElapsed;
        //归零刚开始时的y 
        private float startBackY;
        private AnimationCurve currentCurve;
        //切换曲线过渡中
        private bool transitioning;
        //代入曲线函数求值的自变量
        private float t;

        private float previousY;
        private int movingDirection;//朝正方向运动或静止为0,朝负方向运动为1

        [Header("曲线过渡完成后,从哪些关键帧开始求值,第一个是正方向")]
        [SerializeField]
        private int[] resetKeyFrameIndex;//假设两个曲线大致是一样的

        private void Update()
        {
            RecordDirection();

            if (transitioning)
            {
                BackTransition();
                return;
            }
            this.t += Time.deltaTime;
            ShakeVertical(this.currentCurve, t);
        }
        private void Start()
        {
            currentCurve = null;
            transitioning = false;
            startBackY = 0;
        }
        /// <summary>
        /// 设置震动曲线,将曲线设置为null以暂停
        /// </summary>
        /// <param name="curve"></param>
        public void SetCurve(AnimationCurve curve)
        {
            //切换时,先过渡回0,然后重新引用曲线
            this.currentCurve = curve;
            startBackY = this.transform.localPosition.y;
            ResetTime(curve);
            if (startBackY==0)
            {
                return;
            }
            this.transitioning = true;
            this.backElapsed = 0;
        }
        /// <summary>
        /// 返回预设的曲线
        /// </summary>
        /// <param name="walking"></param>
        /// <returns></returns>
        public AnimationCurve GetCurvePreset(bool walking)
        {
            if (walking)
            {
                return walkCurveVertical;
            }
            else
            {
                return runCurveVertical;
            }
        }
        private void ShakeVertical(AnimationCurve curve,float time)
        {
            if (curve==null)
            {
                return;
            }
            Vector3 newPos= this.transform.localPosition;
            newPos.y = curve.Evaluate(time);
            this.transform.localPosition = newPos;
        }
        private void BackTransition()
        {
            this.backElapsed += Time.deltaTime;
            Vector3 newPos= this.transform.localPosition;
            if (this.backElapsed>backDuration)
            {
                this.transitioning = false;
                newPos.y = 0;
                ResetTime(this.currentCurve);
            }
            else
            {
                newPos.y = Mathf.Lerp(startBackY, 0, backElapsed / backDuration);
            }
            this.transform.localPosition = newPos;
        }
        //重置自变量t
        private void ResetTime(AnimationCurve curve)
        {
            if (curve==null)
            {
                this.t = 0;
                return;
            }
            Keyframe key = curve[resetKeyFrameIndex[movingDirection]];
            this.t = key.time;
        }
        private void RecordDirection()
        {
            float direction=this.transform.localPosition.y-previousY;
            if (direction>=0)
            {
                this.movingDirection = 0;
            }
            else
            {
                this.movingDirection = 1;
            }

            previousY = this.transform.localPosition.y;            
        }
    }
}
