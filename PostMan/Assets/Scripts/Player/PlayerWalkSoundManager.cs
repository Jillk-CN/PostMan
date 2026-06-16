using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.StateMachine;
using PostMan.AudioSystem;
using System.Reflection;

namespace PostMan.Player
{
    public class PlayerWalkSoundManager : MonoBehaviour
    {
        [Header("地面检测")]
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private float groundCheckRadius = 0.3f;

        private PlayerMotion playerMotion;
        private object stateMachine;
        private MethodInfo getStateMethod;
        
        // 备用方案：通过 PlayerMotion 的公开字段获取（如果有）
        private FieldInfo currentStateField;

        [Header("音效")]
        [Tooltip("走路音效播放间隔")]
        public float walkInternetTime = 0.2f;
        [Tooltip("跑步音效播放间隔")]
        public float runInternetTime = 0.01f;
        public AudioClip footStepTileWalk;
        public AudioClip footStepTileRun;
        public AudioClip footStepWoodWalk;
        public AudioClip footStepWoodRun;
        public AudioClip footStepWetRoadWalk;
        public AudioClip footStepWetRoadRun;

        [Header("当前玩家涉足的地面材质")]
        public MaterialState currentMaterial;
        
        private FSMState.StateID currentState = FSMState.StateID.Default;
        private CharacterController characterController;
        private Coroutine _activeMovementCoroutine;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            playerMotion = GetComponent<PlayerMotion>();
            
            if (playerMotion == null)
            {
                Debug.LogError("[PlayerWalkSoundManager] 未找到 PlayerMotion 组件");
                return;
            }
            
            // 获取 stateMachine 字段
            FieldInfo stateMachineField = typeof(PlayerMotion).GetField("stateMachine", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (stateMachineField != null)
            {
                stateMachine = stateMachineField.GetValue(playerMotion);
                
                if (stateMachine != null)
                {
                    
                    getStateMethod = stateMachine.GetType().GetMethod("GetCurrentStateID");
                    
                    if (getStateMethod == null)
                    {
                        Debug.LogWarning("未找到 GetCurrentStateID，尝试其他名称");
                        getStateMethod = stateMachine.GetType().GetMethod("GetCurrentState");
                    }
                    
                    if (getStateMethod == null)
                    {
                        Debug.LogWarning("未找到 GetCurrentState，尝试属性");
                        // 尝试获取 CurrentStateID 属性
                        PropertyInfo prop = stateMachine.GetType().GetProperty("CurrentStateID");
                        if (prop != null)
                        {
                            // 将属性调用包装成方法调用
                            getStateMethod = prop.GetGetMethod();
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("未找到 stateMachine 字段");
            }
            
            // 方案2：尝试获取 PlayerMotion 中的 currentState 字段（DEBUG 模式下的公开字段）
            if (getStateMethod == null)
            {
                currentStateField = typeof(PlayerMotion).GetField("currentState", 
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (currentStateField == null)
                {
                    currentStateField = typeof(PlayerMotion).GetField("currentState", 
                        BindingFlags.NonPublic | BindingFlags.Instance);
                }
                
            }
        }

        private FSMState.StateID GetCurrentPlayerState()
        {
            // 优先使用方法调用
            if (getStateMethod != null && stateMachine != null)
            {
                try
                {
                    return (FSMState.StateID)getStateMethod.Invoke(stateMachine, null);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"方法调用失败: {e.Message}");
                }
            }
            
            // 备用方案：通过字段获取
            if (currentStateField != null && playerMotion != null)
            {
                try
                {
                    return (FSMState.StateID)currentStateField.GetValue(playerMotion);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"字段获取失败: {e.Message}");
                }
            }
            
            return FSMState.StateID.PlayerIdle;
        }

        void Update()
        {
            if (getStateMethod == null && currentStateField == null) return;
            
            FSMState.StateID lastState = GetCurrentPlayerState();

            //弃用地面标签检测
            //currentMaterial = DetectGroundMaterial();

            if (currentState != lastState)
            {
                // 状态变化时，先停止之前的音效
                if (currentState == FSMState.StateID.PlayerWalk || currentState == FSMState.StateID.PlayerRun)
                {
                    AudioManager.Instance.Stop(AudioTrackId.Player, fadeOut: true, fadeOutDuration: 0.1f);
                }
                
                currentState = lastState;

                if (currentState == FSMState.StateID.PlayerIdle)
                {
                    // 空闲状态不播放音效，已经停止了
                }
                else if (currentState == FSMState.StateID.PlayerWalk)
                {
                    PlayWalkSound();
                }
                else if (currentState == FSMState.StateID.PlayerRun)
                {
                    PlayRunSound();
                }
            }
        }

        private void PlayWalkSound()
        {
            // 先停止当前正在运行的协程，确保只有一个协程在运行
            if (_activeMovementCoroutine != null)
            {
                StopCoroutine(_activeMovementCoroutine);
                _activeMovementCoroutine = null;
            }
            
            switch (currentMaterial)
            {
                case MaterialState.Tile:
                    if (footStepTileWalk != null)
                        _activeMovementCoroutine = StartCoroutine(WalkLoopPlay(footStepTileWalk));
                    break;
                case MaterialState.Wood:
                    if (footStepWoodWalk != null)
                        _activeMovementCoroutine = StartCoroutine(WalkLoopPlay(footStepWoodWalk));
                    break;
                case MaterialState.WetRoad:
                    if (footStepWetRoadWalk != null)
                        _activeMovementCoroutine = StartCoroutine(WalkLoopPlay(footStepWetRoadWalk));
                    break;
            }
        }

        private void PlayRunSound()
        {
            // 先停止当前正在运行的协程，确保只有一个协程在运行
            if (_activeMovementCoroutine != null)
            {
                StopCoroutine(_activeMovementCoroutine);
                _activeMovementCoroutine = null;
            }
            
            switch (currentMaterial)
            {
                case MaterialState.Tile:
                    if (footStepTileRun != null)
                        _activeMovementCoroutine = StartCoroutine(RunLoopPlay(footStepTileRun));
                    break;
                case MaterialState.Wood:
                    if (footStepWoodRun != null)
                        _activeMovementCoroutine = StartCoroutine(RunLoopPlay(footStepWoodRun));
                    break;
                case MaterialState.WetRoad:
                    if (footStepWetRoadRun != null)
                        _activeMovementCoroutine = StartCoroutine(RunLoopPlay(footStepWetRoadRun));
                    break;
            }
        }

        /*
        //地面标签射线检测（弃用）
        private MaterialState DetectGroundMaterial()
        {
            Vector3 feetPosition = transform.position;
            if (characterController != null)
            {
                feetPosition = transform.position - new Vector3(0, characterController.height / 2, 0);
            }
            
            if (Physics.SphereCast(feetPosition, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance))
            {
                string tag = hit.collider.tag;
                
                switch (tag)
                {
                    case "Wood":
                        return MaterialState.Wood;
                    case "WetRoad":
                        return MaterialState.WetRoad;
                    case "Tile":
                        return MaterialState.Tile;
                    default:
                        break;
                }
            }
            return currentMaterial;
        }
        */
        
        private void OnDestroy()
        {
            // 清理：停止正在运行的协程
            if (_activeMovementCoroutine != null)
            {
                StopCoroutine(_activeMovementCoroutine);
                _activeMovementCoroutine = null;
            }
            
            // 清理：停止音效
            AudioManager.Instance?.Stop(AudioTrackId.Player, fadeOut: true, fadeOutDuration: 0.1f);
        }

        private IEnumerator WalkLoopPlay(AudioClip moveClip)
        {
            while (currentState == FSMState.StateID.PlayerWalk)
            {
                AudioManager.Instance.Play(AudioTrackId.Player , moveClip);
                yield return new WaitForSeconds(walkInternetTime);
            }
            
            // 协程自然结束时清空引用
            _activeMovementCoroutine = null;
        }

        private IEnumerator RunLoopPlay(AudioClip moveClip)
        {
            while (currentState == FSMState.StateID.PlayerRun)
            {
                AudioManager.Instance.Play(AudioTrackId.Player , moveClip);
                yield return new WaitForSeconds(runInternetTime);
            }
            
            // 协程自然结束时清空引用
            _activeMovementCoroutine = null;
        }
    }

    public enum MaterialState
    {
        Tile,
        Wood,
        WetRoad
    }
}