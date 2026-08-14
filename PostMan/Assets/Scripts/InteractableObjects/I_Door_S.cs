using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using PostMan.AudioSystem;
using PostMan.Scene;
using System;
using UnityEngine.XR;
using UnityEditor.PackageManager;
using PostMan.UI;


namespace PostMan.InteractableObject
{
    public class I_Door_S : MonoBehaviour, IInteractable
    {
        [Header("GameObject")]
        public GameObject blackHand_m;
        public GameObject blackHandPalm_m;
        public GameObject Box;
        public GameObject blackHand_B;
        [Header("当前410门需要执行的动作\n（执行后自动复位，一次交互只能执行一个行为，单选）")]
        [Tooltip("放下包裹，黑手拖包裹")]
        public bool _PlaceBox = false;
        [Tooltip("轻敲门，门后回应")]
        public bool _Knock = false;
        [Tooltip("蹲下查看宠物门")]
        public bool _CheckDoor = false;
        [Header("门状态应用清单")]
        public List<DoorState> doorStateList = new List<DoorState>();

        [Header("音效")]
        [SerializeField] private AudioClip placeGroundSound;
        [SerializeField] private AudioClip dragSlowSound;
        [SerializeField] private AudioClip doorCreakSound;
        [SerializeField] private AudioClip door410KnockSound;
        [SerializeField] private AudioClip door410KnockBackSound;

        [Header("SubtitleKey")]
        public string Day4AMSubtitleKey;

        private Transform SquatCamera; //用于蹲下动作挂载的虚拟相机
        private Transform PlayerCamera; //角色正常移动时的虚拟相机
        private PlayerMotion playerMotion;  //角色移动组件
        private ShowInteractPrompt showInteractPrompt;  //显示交互提示组件
        private Animator cameraAnimator;  //虚拟相机(SquatCamera)上的动画组件
        private Animator doorAnimator;  //宠物门上的动画组件
        private Animator hand_mAnimator;   //黑手上的动画组件
        private Animator hand_BAnimator;
        private Coroutine currentCoroutine;
        private bool isPlaceBoxRunning = false;

        /// <summary>PlaceBox 演出是否正在进行中（供其他组件等待/判断，避免叠加触发）</summary>
        public bool IsPlaceBoxRunning => isPlaceBoxRunning;

        public bool canInteract;
        public int priority;
        public bool CanInteract { get => canInteract; set => canInteract=value; }
        public int Priority { get => priority; set => priority=value; }

        void Start()
        {
            //获取睡觉视角的虚拟相机
            SquatCamera = gameObject.transform.FindChildByName("SquatCamera");
            if(SquatCamera == null)
            {
                Debug.LogError("[I_Door_S.cs] 获取床虚拟相机失败");
            }

            //获取显示交互提示组件
            showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
            if(showInteractPrompt == null)
            {
                Debug.LogError("[I_Door_S.cs] 获取显示交互提示组件失败");
            }

            //获取动画组件
            cameraAnimator = gameObject.transform.FindChildByName("SquatCamera").GetComponent<Animator>();
            doorAnimator = gameObject.transform.FindChildByName("I_D_SD").GetComponent<Animator>();
            hand_mAnimator = blackHand_m.GetComponent<Animator>();
            hand_BAnimator = blackHand_B.GetComponent<Animator>();
            if(cameraAnimator == null || doorAnimator == null || hand_mAnimator == null || hand_BAnimator == null)
            {
                Debug.LogError("[I_Door_S.cs] 获取动画组件失败");
            }

            blackHand_m.SetActive(false);

            blackHand_B.SetActive(false);
        }

        public void InteractWith(PlayerInteractor player)
        {
            if(CanInteract == false) return;
            
            //遍历门状态应用清单，匹配当前场景次序，将对应状态应用到门上
            foreach (DoorState state in doorStateList)
            {
                if (state.sceneOrder == SceneInitializer.Instance.SceneOrder)
                {
                    _PlaceBox = state._PlaceBox;
                    _Knock = state._Knock;
                    _CheckDoor = state._CheckDoor;
                    Debug.Log("410门状态已应用");
                    break;
                }
            }

            if(_PlaceBox)
            {
                StartCoroutine(PlaceBox());

                _PlaceBox = false;

                return;
            }

            if(_Knock)
            {
                StartCoroutine(KnockDoor());

                _Knock = false;

                return;
            }
            
            if(_CheckDoor)
            {
                showInteractPrompt.CanSelect = false;

                //获取玩家正常移动视角的虚拟相机
                PlayerCamera = player.gameObject.transform.FindChildByName("FPVcam");

                //获取角色移动组件
                playerMotion = player.gameObject.GetComponent<PlayerMotion>();

                //暂时禁用角色移动组件
                playerMotion.enabled = false;

                //激活蹲下视角的虚拟相机
                SquatCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

                //失活玩家正常移动视角的虚拟相机，让视角自动过渡到蹲下视角
                PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

                if(currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                currentCoroutine = StartCoroutine(SquatDown());

                _CheckDoor = false;

                return;
            }
        }

        private IEnumerator SquatDown()
        {
            blackHand_B.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            cameraAnimator.SetTrigger("Down");

            yield return new WaitForSeconds(2f);

            cameraAnimator.ResetTrigger("Down");
            doorAnimator.SetTrigger("Open");

            AudioManager.Instance.Play(AudioTrackId.FX , doorCreakSound);

            yield return new WaitForSeconds(1.2f);

            hand_BAnimator.SetTrigger("Search");

            yield return new WaitForSeconds(4.1f);

            BlackScreen.Instance.BlackInOut("" , 0.1f , 0.8f , 0.1f);

            yield return new WaitForSeconds(1f);

            hand_BAnimator.ResetTrigger("Search");

            doorAnimator.ResetTrigger("Open");
            doorAnimator.SetTrigger("Close");

            yield return new WaitForSeconds(3f);

            SubtitleUI.Instance.TypeSubtitle(Day4AMSubtitleKey);

            doorAnimator.ResetTrigger("Close");

            blackHand_B.SetActive(false);

            cameraAnimator.SetTrigger("Up");

            yield return new WaitForSeconds(1f);

            cameraAnimator.ResetTrigger("Up");

            //重新启用允许交互
            showInteractPrompt.CanSelect = true;

            //激活玩家正常移动视角的虚拟相机
            PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

            //失活蹲下视角的虚拟相机，让视角自动过渡到玩家正常视角
            SquatCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

            //启用角色移动组件
            playerMotion.enabled = true;

        }

        /// <summary>
        /// 在410门前放置箱子
        /// </summary>
        private IEnumerator PlaceBox()
        {
            isPlaceBoxRunning = true;

            try
            {
                blackHand_m.SetActive(true);

                Box.SetActive(true);

                AudioManager.Instance.Play(AudioTrackId.FX , placeGroundSound);

                yield return new WaitForSeconds(1f);

                doorAnimator.SetTrigger("Open");

                AudioManager.Instance.Play(AudioTrackId.FX , doorCreakSound);

                yield return new WaitForSeconds(1.5f);

                doorAnimator.ResetTrigger("Open");

                //黑手拖包裹动作
                //先清除可能残留的 trigger，避免上一次未消费的 Drap flag 导致动画自动重播
                hand_mAnimator.ResetTrigger("Drap");
                hand_mAnimator.SetTrigger("Drap");

                yield return new WaitForSeconds(2.11f);

                Box.transform.parent = blackHandPalm_m.transform;

                AudioManager.Instance.Play(AudioTrackId.FX , dragSlowSound);

                yield return new WaitForSeconds(3f);

                //Drap 片段此时已播放完毕，再清一次，防止状态机回到默认状态后残留 flag 再次触发
                hand_mAnimator.ResetTrigger("Drap");

                doorAnimator.SetTrigger("Close");

                yield return new WaitForSeconds(3f);

                doorAnimator.ResetTrigger("Close");

                Box.transform.parent = gameObject.transform;

                Box.SetActive(false);

                blackHand_m.SetActive(false);
            }
            finally
            {
                isPlaceBoxRunning = false;
            }
        }

        private IEnumerator KnockDoor()
        {
            //敲门

            AudioManager.Instance.Play(AudioTrackId.FX , door410KnockSound);

            yield return new WaitForSeconds(2f);

            AudioManager.Instance.Play(AudioTrackId.FX , door410KnockBackSound);
        }
    }

    [Serializable]
    public class DoorState
    {
        public int sceneOrder = -1;
        public bool _PlaceBox = false;
        public bool _Knock = false;
        public bool _CheckDoor = false;
    }
}
