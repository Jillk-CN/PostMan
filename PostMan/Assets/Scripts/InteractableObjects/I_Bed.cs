using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using UnityEngine.UI;
using Cinemachine;


/// <summary>
/// 睡觉动画：
/// 通过激活/失活 床上睡觉(SleepCamera) 和 玩家移动(FPVcam) 的两个虚拟相机，让视角过渡到展示躺下过程的相机
/// 完成休息任务：
/// 遍历休息任务索引，找到正在进行的休息任务，推进该休息任务 
/// </summary>
public class I_Bed : MonoBehaviour, IInteractable
{
    [Header("躺下后黑屏渐变设置")]
    [Tooltip("黑屏UI图像，拖拽赋值，UI路径Canvas/BlackScreen")]
    [SerializeField] private Image BlackScreen;   //黑屏UI图像
    [Tooltip("渐变时长")]
    [SerializeField] private float FadeTime;    //渐变时长

    [Header("音效")]
    [Tooltip("躺下音效")]
    [SerializeField] private AudioClip SleepSound;
    [Tooltip("起床音效")]
    [SerializeField] private AudioClip WakeUpSound;
    private AudioSource audioSource;

    [Header("休息任务索引(填Current Index)及下一任务SO(填Next Task)列表")]
    [SerializeField] private List<TaskTransition> taskTransitionList;

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

        //获取音效组件
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            Debug.LogError("[I_Bed.cs] 获取音效组件失败");
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Bed.cs] 与床交互");

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

        //重新启用允许交互
        showInteractPrompt.CanSelect = true;

        //激活玩家正常移动视角的虚拟相机
        PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

        //失活睡觉视角的虚拟相机，让视角自动过渡到玩家正常视角
        SleepCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

        //启用角色移动组件
        playerMotion.enabled = true;
    }

    private IEnumerator Sleep()
    {

        //执行躺下动作
        animator.SetTrigger("Sleep");

        //播放躺下音效
        if(SleepSound != null)
        {
            audioSource.PlayOneShot(SleepSound);
        }

        yield return new WaitForSeconds(3.5f);

        animator.ResetTrigger("Sleep");
        BlackScreen.gameObject.SetActive(true); //激活黑屏UI

        //强制完全透明
        Color color = BlackScreen.color;
        color.a = 0;
        BlackScreen.color = color;
        float Timer = 0f;

        //黑屏渐入
        while(BlackScreen.color.a < 1f)
        {
            Timer += Time.deltaTime;

            color.a = Mathf.Lerp(0f , 1f , Timer/FadeTime);

            BlackScreen.color = color;

            yield return null;
        }

        //强制完全黑屏
        color.a = 1;
        BlackScreen.color = color;

        Sleeping = true;
    }

    private IEnumerator WakeUp()
    {
        
        Color color = BlackScreen.color;
        //强制完全黑屏
        color.a = 1;    
        BlackScreen.color = color;
        float Timer = 0f;

        //黑屏渐出
        while(BlackScreen.color.a > 0f)
        {
            Timer += Time.deltaTime;

            color.a = Mathf.Lerp(1f , 0f , Timer/FadeTime);

            BlackScreen.color = color;

            if(Timer > FadeTime)
            {

                break;
            }

            yield return null;
        }

        //强制完全透明
        color.a = 0;
        BlackScreen.color = color;

        BlackScreen.gameObject.SetActive(false); //失活黑屏UI

        //执行起身动作
        animator.SetTrigger("WakeUp");

        //播放起床音效
        if(WakeUpSound != null)
        {
            audioSource.PlayOneShot(WakeUpSound);
        }

        yield return new WaitForSeconds(3.5f);

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
                TaskManager.Instance.AdvanceTask(task.currentIndex , 1);

                TaskManager.Instance.StartTask(task.nextTask);
            }
        }
    }
    
}
