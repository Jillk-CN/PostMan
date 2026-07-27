using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using UnityEngine;
using UnityEngine.UI;
using PostMan.InputManagement;

public class Forum : MonoBehaviour
{
    [Header("所属任务的索引 / 下一任务SO文件")]
    [SerializeField]
    private TaskTransition taskTransition;

    [Header("帖子列表")]
    [SerializeField]
    private List<ForumPost> forumPosts;

    [Header("字幕Key列表")]
    public string subtitleAfterPost1;
    public string subtitleAfterPost2;
    public string subtitleAfterPost3;
    public string subtitleAfterClose;
    
    [Header("交互物PC，手动拖拽")]
    [SerializeField]
    private GameObject PC;
    public AudioClip clickSound;
    public AudioClip returnSound;
    private GameInputManager gameInputManager;

    void Start()
    {
        // 游戏开始时隐藏论坛UI
        transform.localScale = Vector3.zero;

        for (int i = 0; i < forumPosts.Count; i++)
        {
            int index = i;
            
            if (forumPosts[i].OpenButton != null)
            {
                forumPosts[i].OpenButton.onClick.AddListener(() => OpenPost(index));
            }
            
            if (forumPosts[i].CloseButton != null)
            {
                forumPosts[i].CloseButton.onClick.AddListener(() => ClosePost(index));
            }
        }

        gameInputManager = FindAnyObjectByType<GameInputManager>();
    }

    private void OpenPost(int postIndex)
    {
        ForumPost post = forumPosts[postIndex];
        
        if (post.postUI != null)
        {
            post.postUI.SetActive(true);
        }

        if(clickSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , clickSound , false , false , 0f , 1f);
        }
        
        Debug.Log($"打开帖子 {postIndex}");
    }

    private void ClosePost(int postIndex)
    {
        ForumPost post = forumPosts[postIndex];
        
        if(returnSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , returnSound , false , false , 0f , 1f);
        }

        if (post.postUI != null)
        {
            post.postUI.SetActive(false);
        }
        
        if (!post.readed)
        {
            post.readed = true;
            Debug.Log($"帖子 {postIndex} 已标记为已读");
        }
        
        Debug.Log($"关闭帖子 {postIndex}");
        
        // 检查所有帖子是否都已读
        CheckAllPostsReaded();
    }
    
    private void CheckAllPostsReaded()
    {
        foreach (ForumPost post in forumPosts)
        {
            if (!post.readed)
            {
                return;  // 还有未读，不关闭
            }
        }
        
        // 所有帖子都已读，关闭论坛
        Debug.Log("所有帖子已读，关闭论坛");
        CloseForum();
    }
    
    /// <summary>
    /// 关闭论坛并重置所有帖子的已读状态
    /// </summary>
    private void CloseForum()
    {
        // 重置所有帖子的已读状态为未读
        ResetAllPostsReadStatus();
        
        // 调用 PC 的关闭方法
        if (PC != null)
        {
            PC.GetComponent<I_PC>().ClosePC();
        }

        if(TaskManager.Instance.IsTaskActive(taskTransition.currentIndex))
        {
            TaskManager.Instance.AdvanceTask(taskTransition.currentIndex);

            TaskManager.Instance.StartTask(taskTransition.nextTask);

        }

        SubtitleUI.Instance.TypeSubtitle(subtitleAfterClose);

        //等待所有组件完成过渡后再确保恢复移动
        StartCoroutine(DelayedRestoreMovement());
    }

    private IEnumerator DelayedRestoreMovement()
    {
        // 等待帧末，让所有事件和组件在这一帧内完成处理
        yield return new WaitForEndOfFrame();

        // 再等待一帧，确保场景加载等异步操作触发的逻辑已完成
        yield return null;

        if (gameInputManager != null)
        {
            gameInputManager.SetPlayerAllInput(true);
        }
    }
    
    /// <summary>
    /// 重置所有帖子的已读状态为未读
    /// </summary>
    private void ResetAllPostsReadStatus()
    {
        foreach (ForumPost post in forumPosts)
        {
            post.readed = false;
        }
        Debug.Log("所有帖子的已读状态已重置为未读");
    }
    
    /// <summary>
    /// 手动关闭论坛（外部调用）
    /// </summary>
    public void ManualCloseForum()
    {
        if(TaskManager.Instance.IsTaskActive(taskTransition.currentIndex))
        {
            TaskManager.Instance.AdvanceTask(taskTransition.currentIndex);

            TaskManager.Instance.StartTask(taskTransition.nextTask);

        }

        CloseForum();
    }

    public void TypeSubtitleAfterPost1()
    {
        SubtitleUI.Instance.TypeSubtitle(subtitleAfterPost1);
    }
    public void TypeSubtitleAfterPost2()
    {
        SubtitleUI.Instance.TypeSubtitle(subtitleAfterPost2);
    }
    public void TypeSubtitleAfterPost3()
    {
        SubtitleUI.Instance.TypeSubtitle(subtitleAfterPost3);
    }
}