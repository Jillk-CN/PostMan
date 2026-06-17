using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using UnityEngine.XR;
using PostMan.Common;
using PostMan.AudioSystem;

public class I_MailBox : MonoBehaviour, IInteractable
{
    [Header("是否为410信箱")]
    [SerializeField] private bool is410MailBox = false;
    [Header("音效")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip takeLetterSound;
    [SerializeField] private AudioClip insertLetterSound;
    [SerializeField] private AudioClip mailbox410OpenSound;
    [SerializeField] private AudioClip mailbox410PopSound;
    [SerializeField] private AudioClip mailbox410HitInsideSound;

    [Header("每日普通任务索引列表")]
    [Tooltip("填入每日普通任务的任务索引 以及 下一任务的SO文件")]
    [SerializeField] private List<TaskTransition> taskTransitionList;

    private Animator handleAnimator;
    private Animator doorAnimator;
    private bool isMail = false;
    private ShowInteractPrompt showInteractPrompt;  //显示交互提示组件

    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    void Start()
    {
        //获取显示交互提示组件
        showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
        if(showInteractPrompt == null)
        {
            Debug.LogError("[I_MailBox.cs] 获取显示交互提示组件失败");
        }
        
        handleAnimator = gameObject.transform.FindChildByName("I_MB_H").GetComponent<Animator>();
        doorAnimator = gameObject.transform.FindChildByName("I_MB_D").GetComponent<Animator>();
        if(handleAnimator == null || doorAnimator == null)
        {
            Debug.LogError("[I_MailBox.cs] 获取动画组件失败");
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_MailBox.cs] 与信箱交互");

        StartCoroutine(Door());
    }

    private IEnumerator Door()
    {
        showInteractPrompt.CanSelect = false;

        doorAnimator.SetTrigger("Open");

        if(openSound != null && !is410MailBox)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , openSound);
        }
        else if(openSound != null && is410MailBox)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , mailbox410OpenSound);
        }

        yield return new WaitForSeconds(1f);

        doorAnimator.ResetTrigger("Open");

        yield return StartCoroutine(Handle());

        AdvanceRestTask();

        doorAnimator.SetTrigger("Close");

        if(closeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , closeSound);
        }

        yield return new WaitForSeconds(0.3f);

        doorAnimator.ResetTrigger("Close");
        
        showInteractPrompt.CanSelect = true;
    }

    private IEnumerator Handle()
    {
        if(isMail == false)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , insertLetterSound);

            yield return new WaitForSeconds(0.7f);

            handleAnimator.SetTrigger("Up");

            isMail = true;
        }
        else if(isMail)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , takeLetterSound);

            yield return new WaitForSeconds(0.7f);

            handleAnimator.SetTrigger("Down");

            isMail = false;
        }

        yield return new WaitForSeconds(0.5f);

        handleAnimator.ResetTrigger("Up");
        handleAnimator.ResetTrigger("Down");
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

    public void MailBox410Pop()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , mailbox410PopSound);
    }

    public void MailBox410HitInside()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , mailbox410HitInsideSound);
    }
}
