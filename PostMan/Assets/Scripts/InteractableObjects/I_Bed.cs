using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using UnityEngine.UI;
using Cinemachine;
using PostMan.UI;
using PostMan.AudioSystem;
using System;
using PostMan.Scene;


namespace PostMan.InteractableObject
{
    /// <summary>
    /// 睡觉动画：
    /// 通过激活/失活 床上睡觉(SleepCamera) 和 玩家移动(FPVcam) 的两个虚拟相机，让视角过渡到展示躺下过程的相机
    /// 完成休息任务：
    /// 遍历休息任务索引，找到正在进行的休息任务，推进该休息任务 
    /// </summary>
    public class I_Bed : MonoBehaviour, IInteractable
    {
        [Header("音效")]
        [Tooltip("躺下音效")]
        [SerializeField] private AudioClip SleepSound;
        [Tooltip("起床音效")]
        [SerializeField] private AudioClip WakeUpSound;

        [Header("休息任务索引(填Current Index)及下一任务SO(填Next Task)列表")]
        [SerializeField] private List<TaskTransition> taskTransitionList;

        [Header("休息任务索引(填Current Index)及起床后要触发的字幕Key")]
        [SerializeField] private List<TaskPromptSubtitle> taskPromptSubtitles;
        private int currentIndex;

        private Transform SleepCamera; //用于睡觉动作挂载的虚拟相机
        private Transform PlayerCamera; //角色正常移动时的虚拟相机
        private PlayerMotion playerMotion;  //角色移动组件
        private ShowInteractPrompt showInteractPrompt;  //显示交互提示组件
        private Animator animator;  //虚拟相机(SleepCamera)上的动画组件
        private bool Sleeping = false;
        private Coroutine currentCoroutine = null;
        
        [Header("交互设置")]
        public bool canInteract;
        public int priority;
        public bool CanInteract { get => canInteract; set => canInteract=value; }
        public int Priority { get => priority; set => priority=value; }

        void Start()
        {
            //获取睡觉视角的虚拟相机
            SleepCamera = gameObject.transform.FindChildByName("SleepCamera");
            if(SleepCamera == null)
            {
                Debug.LogError("[I_Bed.cs] 获取床虚拟相机失败");
            }

            //获取显示交互提示组件
            showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
            if(showInteractPrompt == null)
            {
                Debug.LogError("[I_Bed.cs] 获取显示交互提示组件失败");
            }

            //获取动画组件
            animator = gameObject.transform.FindChildByName("SleepCamera").GetComponent<Animator>();
            if(animator == null)
            {
                Debug.LogError("[I_Bed.cs] 获取动画组件失败");
            }
        }

        public void InteractWith(PlayerInteractor player)
        {
            if(CanInteract == false) return;

            //获取玩家正常移动视角的虚拟相机
            PlayerCamera = player.gameObject.transform.FindChildByName("FPVcam");

            //获取角色移动组件
            playerMotion = player.gameObject.GetComponent<PlayerMotion>();

            //暂时禁用角色移动组件
            playerMotion.enabled = false;

            //激活睡觉视角的虚拟相机
            SleepCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

            //失活玩家正常移动视角的虚拟相机，让视角自动过渡到睡觉视角
            PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

            if(currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(BedBehaviour());

        }

        private IEnumerator BedBehaviour()
        {
            Coroutine currentSleepCoroutine = null;
            Coroutine currentWakeUpCoroutine = null;

            //暂时禁用交互，避免播放动画时，因为交互检测显示的交互按钮
            showInteractPrompt.CanSelect = false;

            if(Sleeping == false)
            {
                if(currentSleepCoroutine != null)
                {
                    StopCoroutine(currentSleepCoroutine);
                }
                yield return currentSleepCoroutine = StartCoroutine(Sleep());
            }

            //推进时间（未完成，等待推进时间接口）
            //进入下一时段（未完成）

            //完成休息任务 并 开启下一任务
            AdvanceRestTask();

            if(Sleeping == true)
            {
                if(currentWakeUpCoroutine != null)
                {
                    StopCoroutine(currentWakeUpCoroutine);
                }
                yield return currentWakeUpCoroutine = StartCoroutine(WakeUp());
            }

            //激活玩家正常移动视角的虚拟相机
            PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

            //失活睡觉视角的虚拟相机，让视角自动过渡到玩家正常视角
            SleepCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

            //启用角色移动组件
            playerMotion.enabled = true;

            //更新交互提示
            UpdateInteractPrompt();
        }

        private IEnumerator Sleep()
        {

            //执行躺下动作
            animator.SetTrigger("Sleep");

            //播放躺下音效
            if(SleepSound != null)
            {
                AudioManager.Instance.Play(AudioTrackId.FX , SleepSound , false , false , 0f , 1f);
            }

            yield return new WaitForSeconds(3.5f);

            animator.ResetTrigger("Sleep");

            yield return new WaitForSeconds(3.5f);
        
            // 黑屏淡入
            BlackScreen.Instance.BlackIn("", 2f);

            Sleeping = true;
        }

        private IEnumerator WakeUp()
        {
            // 黑屏淡出
            BlackScreen.Instance.BlackOut("", 2f);

            //执行起身动作
            animator.SetTrigger("WakeUp");

            //播放起床音效
            if(WakeUpSound != null && SceneInitializer.Instance.SceneOrder != 19)
            {
                AudioManager.Instance.Play(AudioTrackId.FX , WakeUpSound , false , false , 0f , 1f);
            }

            yield return new WaitForSeconds(3.5f);

            PlaySubtitle(currentIndex);

            animator.ResetTrigger("WakeUp");

            Sleeping = false;
        }

        //完成当前休息任务并开启下一任务
        private void AdvanceRestTask()
        {
            foreach(var task in taskTransitionList)
            {
                if(TaskManager.Instance.IsTaskActive(task.currentIndex))
                {
                    currentIndex = task.currentIndex;

                    TaskManager.Instance.AdvanceTask(task.currentIndex , 1);

                    TaskManager.Instance.StartTask(task.nextTask);

                    break;
                }
            }
        }

        //根据新任务的索引启用对应起床字幕
        private void PlaySubtitle(int Index)
        {
            foreach(var subtitle in taskPromptSubtitles)
            {
                if(subtitle.currentIndex == Index)
                {
                    SubtitleUI.Instance.TypeSubtitle(subtitle.subtitleKey);

                    break;
                }
            }
        }

        private void UpdateInteractPrompt()
        {
            showInteractPrompt.CanSelect = canInteract;
        }
        
    }


    [Serializable]
    class TaskPromptSubtitle
    {
        public int currentIndex;

        public string subtitleKey;
    }


}