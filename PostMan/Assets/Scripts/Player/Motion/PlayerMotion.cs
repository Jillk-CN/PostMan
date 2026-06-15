using PostMan.InputManagement;
using PostMan.StateMachine;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace PostMan.Player
{
    /*
        重构思路:
    用状态机控制状态之间的切换
    其他类<-----PlayerMotion-----> 状态机
    一方面,让PlayerMotion向外部的类提供状态信息,比如玩家是在移动、奔跑等等
    另一方面,PlayerMotion提供状态机需要调用的方法和需要的数据,让状态类里能够执行真正的逻辑
     */

    /// <summary>
    /// 玩家移动类,实现移动的功能,提供玩家移动的相关数据
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotion : MonoBehaviour
    {
#if DEBUG
        public FSMState.StateID currentState;
#endif
        //输入源
        [HideInInspector]
        private PlayerMotionInputSource motionInput;
        private PlayerSight sight;
        private MonoFSM stateMachine;
        #region 移动相关字段
        private CharacterController characterController;

        [Header("移动设置")]
        [Tooltip("行走速度")]
        public float walkSpeed;
        [Tooltip("奔跑速度倍率 (相对于行走速度)")]
        public float runMultiplier;
       // [Tooltip("蹲伏速度倍率 (相对于行走速度)")]
       // public float crouchMultiplier;
        [Tooltip("是否在不移动的时候也应用重力")]
        public bool autoGravity;
        #endregion

        // [Header("脚步音效")]
        // [SerializeField] private AudioClip footstepClip;
        //[SerializeField] private float runPitchMultiplier = 1.5f;
        //private bool _footstepPlaying;

        /*
         
        #region 下蹲相关字段
        [Header("下蹲设置")]
        [Tooltip("站立高度")]
        public float standingHeight = 2f;
        [Tooltip("蹲伏高度")]
        public float crouchingHeight = 1f;
        [Tooltip("下蹲过渡平滑速度")]
        [SerializeField]
        private float crouchTransitionSpeed = 10f;
        //是否在过渡中
        private bool heightChanging;

        [Tooltip("头顶检测距离 (防止在障碍物下站起)")]
        [SerializeField]
        private float headCheckDistance = 0.5f;
        [Tooltip("头顶检测层级")]
        [SerializeField]
        private LayerMask obstacleLayer;
        //下蹲&起立要到的高度
        private float targetHeight;
        [Tooltip("相机距离头顶的偏移量")]
        [SerializeField]
        private float sightOffset=-0.4f;
        #endregion
         */


        private void Awake()
        {
            // 防御性修复：与 PlayerSight.Awake() 保持一致，确保输入源在 Start() 前就绪
            motionInput = GameInputManager.Instance.GetInputSystemSource<PlayerMotionInputSource>();
            characterController = this.GetComponent<CharacterController>();
        }

        private void Start()
        {
            // sight 在 Start() 中获取，此时 PlayerSight.Awake() 必然已执行，sightInput 已就绪
            sight = this.GetComponentInChildren<PlayerSight>();
            //targetHeight = standingHeight;
           // this.heightChanging = false;
            initStateMachine();
        }

        private void Update()
        {
            currentState = stateMachine.GetCurrentStateID();
            stateMachine.Execute();
        }

        public void Move(Vector3 direction,float speed)
        {
            if (direction==Vector3.zero||speed<=0)
            {
                return;
            }
            direction.Normalize();
            Vector3 finalMove =
                direction * speed * Time.deltaTime;
            characterController.Move(finalMove);
        }

        /// <summary>
        /// 玩家旋转
        /// </summary>
        /// <param name="rotateDirection">视线的移动方向</param>
        /// <param name="sensitivity">灵敏度</param>
        public void Rotate(float rotateDirection,float sensitivity)
        {
            //float playerRotationAngle = rotateDirection.x * sensitivity * Time.deltaTime;
            float playerRotationAngle = rotateDirection * sensitivity * Time.deltaTime;
            //由于水平方向没有限制,随便旋转都行,在当前的rotation上转即可
            this.transform.Rotate(Vector3.up * playerRotationAngle); 

        }
        public PlayerMotionInputSource GetInputSource()
        {
            return this.motionInput;
        }

        /// <summary>
        /// 强制朝向指定地点,只在禁止玩家输入的时候才调用这个方法!
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <param name="duration">转向总时间</param>
        public void LookAt(Vector3 target, float duration)
        {
            StartCoroutine(LookAtTransition(target, duration));
        }

        /// <summary>
        /// 强制转向的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator LookAtTransition(Vector3 target,float duration)
        {

            float t = 0;
            Quaternion startBodyRot = this.transform.rotation;
            Quaternion startCamRot = sight.transform.localRotation;

            while (t < duration)//这个暂时硬编码一个持续时间1
            {
                t += Time.deltaTime;
                //要朝向的方向
                Vector3 direction = (target - sight.transform.position).normalized;
                
                if (direction != Vector3.zero) 
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    Vector3 euler = lookRot.eulerAngles;
                    
                    //身体只转y轴
                    Quaternion targetBodyRot = Quaternion.Euler(0, euler.y, 0);
                        this.transform.rotation = Quaternion.Slerp
                             (startBodyRot, targetBodyRot, t/duration);

                    //相机只转x轴
                    float targetX = euler.x;
                    Math.Clamp(targetX, sight.sightAngleMin, sight.sightAngleMax);
                    Quaternion targetCamRot = Quaternion.Euler(targetX, 0, 0);
                    Quaternion localRotation =
                        Quaternion.Slerp(startCamRot, targetCamRot, t/duration);
                    //同步旋转
                    sight.Rotate(localRotation.eulerAngles.x);
                }

                 yield return null;
             }

        }

        private void initStateMachine()
        {
            //暂时先手动硬编码状态机
            stateMachine = new MonoFSM(this.gameObject);
            //待机
            FSMState state = new PlayerIdleState(stateMachine);
            state.AddTransition(FSMCondition.ConditionID.PlayerWalk,
                FSMState.StateID.PlayerWalk);
            stateMachine.AddState(state);


            //移动
            state = new PlayerWalkState(stateMachine);
            state.AddTransition(FSMCondition.ConditionID.PlayerStopWalk,
                FSMState.StateID.PlayerIdle);
            state.AddTransition(FSMCondition.ConditionID.PlayerRun,
                FSMState.StateID.PlayerRun);
            stateMachine.AddState(state);

            //奔跑
            state = new PlayerRunState(stateMachine);
            state.AddTransition(FSMCondition.ConditionID.PlayerStopRun,
                FSMState.StateID.PlayerWalk);
            stateMachine.AddState(state);

            /*
             
            //下蹲待机
            state = new PlayerCrouchedIdleState(stateMachine);
            state.AddTransition(FSMCondition.ConditionID.PlayerWalk,
                FSMState.StateID.PlayerCrouchedWalk);
            compositeCondition.AddCondition(new PlayerSwitchCrouchCondition());
            compositeCondition.AddCondition(new PlayerCanStandCondition());
            state.AddTransition(compositeCondition,
                FSMState.StateID.PlayerIdle);
            stateMachine.AddState(state);

            //下蹲行走
            state = new PlayerCrouchedWalkState(stateMachine);
            state.AddTransition(FSMCondition.ConditionID.PlayerStopWalk,
                FSMState.StateID.PlayerCrouchedIdle);
            compositeCondition = new CompositeCondition();
            compositeCondition.AddCondition(new PlayerSwitchCrouchCondition());
            compositeCondition.AddCondition(new PlayerCanStandCondition());
            state.AddTransition(compositeCondition,
                FSMState.StateID.PlayerWalk);
            stateMachine.AddState(state);

             */

            stateMachine.SetDefaultState(FSMState.StateID.PlayerIdle);
        }
    }
}
//一些废弃代码
    /*
     
        private void InitHeight()
        {
            characterController.height = standingHeight;
            //保证玩家空物体在脚底
            characterController.center = new Vector3(0, standingHeight / 2, 0);
            //假设相机在头顶的位置
            sight.transform.localPosition = new Vector3
                (0, standingHeight+sightOffset, 0);
        }
     */
        /*
        public void ForceMoveTo(Vector3 targetPos,float duration,Action arrived)
        {
            StartCoroutine(ForceMove(targetPos,duration, arrived)); 
        } 
        private IEnumerator ForceMove(Vector3 targetPos,float duration,Action arrived)
        {
            float elpased = 0;
            Vector3 startPos = this.transform.position;
            while (elpased<duration)
            {
                elpased += Time.deltaTime;
                this.transform.position = Vector3.Lerp
                    (startPos, targetPos, elpased / duration);
                
                sight.ApplyRunShake();
                yield return null;
            }
            this.transform.position = targetPos;
            arrived?.Invoke();
        }

         
         */


        /*
         
        public void ChangeHeight(float targetHeight)
        {
            this.heightChanging = true;
            this.targetHeight = targetHeight;
        }
         */

        /*
         
        /// <summary>
        /// 玩家是否就能站起来
        /// </summary>
        /// <returns></returns>
        public bool CanStand()
        {
            /*测试代码,记得删
             
            RaycastHit hit;
             bool has= Physics.BoxCast(this.transform.position, this.transform.localScale * 0.5f,
                    this.transform.up,out hit ,Quaternion.identity,
                    headCheckDistance + standingHeight,obstacleLayer);
            if (has)
            {
                Debug.LogWarning("头上是"+hit.collider.transform.name);
            }
            return has;
            return !Physics.BoxCast(this.transform.position, this.transform.localScale * 0.5f,
                    this.transform.up, Quaternion.identity,
                    headCheckDistance + standingHeight,obstacleLayer);
        }
         */

        /*
         
        /// <summary>
        /// 实现下蹲时的高度的平滑过渡
        /// </summary>
        private void ChangingHeight()
        {
            characterController.height =
                Mathf.Lerp(characterController.height, targetHeight,
                crouchTransitionSpeed * Time.deltaTime);
            //相等后就停止过渡
            float acceptableDelta = 0.01f;
            if (Mathf.Abs(characterController.height-targetHeight)<acceptableDelta)
            {
                heightChanging = false;
            }
        
            // 修正中心点，防止穿地
            Vector3 center = characterController.center;
            center.y = characterController.height / 2f;
            characterController.center = center;
            //暂时这里直接操控相机,让相机的位置跟着下降
            //假定让相机在CharacterController的顶部
            Vector3 sightPos = sight.transform.localPosition;
            sightPos.y = characterController.height+sightOffset;
            sight.transform.localPosition = sightPos;
        }
         */
