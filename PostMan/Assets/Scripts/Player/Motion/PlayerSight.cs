using PostMan.InputManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Player
{
    /// <summary>
    /// 玩家视野,实现相机的旋转和震动
    /// </summary>
    public class PlayerSight : MonoBehaviour
    {
        #region 旋转相关设置
        /// <summary>
        ///是否允许旋转 
        /// </summary>
        public bool enableShake=false;
        [Header("相机旋转相关设置")]
        [Tooltip("相机最小旋转角度")]
        [SerializeField]
        public float sightAngleMax;
        [Tooltip("相机最小旋转角度")]
        [SerializeField]
        public float sightAngleMin;
        [Tooltip("旋转灵敏度")]
        public float sensitivity;
        /// <summary>
        /// 累计旋转的角度
        /// </summary>
        private float cameraRotationAngle=0;
        #endregion
        private PlayerSightInputSource sightInput;
        #region 震动相关设置
        [Header("震动效果相关设置")]
        private SinShakeEffect shakeEffect;
        [Header("移动时的震动")]
        [Tooltip("走路震动振幅")]
        [SerializeField]
        private float amplitudeWalk;
        [Tooltip("走路震动频率")]
        [SerializeField]
        private float frequencyWalk;
        [Header("跑步时的震动")]
        [Tooltip("跑步震动振幅")]
        [SerializeField]
        private float amplitudeRun;
        [Tooltip("跑步震动频率")]
        [SerializeField]
        private float frequencyRun;
        #endregion
        private void Start()
        {
            sightInput= GameInputManager.Instance.GetInputSystemSource<PlayerSightInputSource>();
            shakeEffect = this.GetComponentInChildren<SinShakeEffect>();
            // 从 PlayerPrefs 读取标题场景保存的设置，缺省保留 Inspector 配置值
            sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", sensitivity);
            enableShake = PlayerPrefs.GetInt("EnableShake", enableShake ? 1 : 0) == 1;
        }

        /// <summary>
        /// 相机随着视线旋转
        /// </summary>
        /// <param name="direction">视线移动的方向</param>
        /// <param name="sensitivity">灵敏度</param>
        public void Rotate(float direction, float sensitivity)
        {
            //cameraRotationAngle -= direction.y * sensitivity * Time.deltaTime;
            cameraRotationAngle -= direction * sensitivity * Time.deltaTime;
            cameraRotationAngle = Mathf.Clamp(cameraRotationAngle, sightAngleMin, sightAngleMax);

            this.transform.localRotation = Quaternion.Euler
                (new Vector3(cameraRotationAngle, 0, 0));
        }
        /// <summary>
        /// 让相机记录并旋转至指定角度
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(float angle)
        {
            //因为转过一周的效果其实是一样的,还得
            if (angle>=sightAngleMax)
            {
                angle -= 360;
            }
            else if(angle<=sightAngleMin)
            {
                angle += 360;
            }
            cameraRotationAngle = angle;
           //cameraRotationAngle = Mathf.Clamp(cameraRotationAngle, sightAngleMin, sightAngleMax);
            this.transform.localRotation = Quaternion.Euler(new Vector3(cameraRotationAngle, 0, 0));
            
        }

        //下面的这些震动方法在对应运动状态开始时调用一次即可,否则会出现一些问题

        /// <summary>
        ///应用走路时的震动 
        /// </summary>
        public void ApplyWalkShake()
        {
            shakeEffect.amplitude = amplitudeWalk;
            shakeEffect.SetFrequency(frequencyWalk);
            shakeEffect.StartShake();
            //shakeEffect.SetNoise(amplitudeWalk, frequencyWalk);
        }
        /// <summary>
        /// 应用跑步时的震动
        /// </summary>
        public void ApplyRunShake()
        {
           // hasShake = true;
            shakeEffect.amplitude = amplitudeRun;
            shakeEffect.SetFrequency(frequencyRun);
            shakeEffect.StartShake();
            //shakeEffect.SetNoise(amplitudeRun, frequencyRun);
        }
        /// <summary>
        /// 停止震动
        /// </summary>
        public void StopShake()
        {
            // hasShake = false;
            shakeEffect.StopShake();
            //shakeEffect.Mute();
        }

        /// <summary>
        /// 设置水平的震动的偏移方向,否则默认向左边偏移
        /// 暂时将这个方法放在这里
        /// </summary>
        /// <param name="moveVector">输入里的移动方向</param>
        public void SetHorizontalShakeDirection(Vector3 moveVector)
        {
            if (moveVector.x>0)
            {
                shakeEffect.SetHorizontalDirection(SinShakeEffect.LEFT);
            }
            else if(moveVector.x<0)
            {
                shakeEffect.SetHorizontalDirection(SinShakeEffect.RIGHT);
            }
            else
            {
                shakeEffect.SetHorizontalDirection(0);
            }
        }

        /// <summary>
        /// 禁用旋转
        /// </summary>
        public void DisableShake()
        {
            shakeEffect.StopShake();
            //shakeEffect.Mute(true);
            this.enableShake = false;    
        }
        /// <summary>
        /// 开启旋转
        /// </summary>
        public void EnableShake()
        {
            this.enableShake = true;
        }
        public PlayerSightInputSource GetInputSource()
        {
            return this.sightInput;
        }
    }
}
