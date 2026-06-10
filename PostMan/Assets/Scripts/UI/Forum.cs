using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Forum : MonoBehaviour
{
    [Header("所属任务的索引 / 下一任务SO文件")]
    [SerializeField]
    private TaskTransition taskTransition;

    [Header("帖子列表")]
    [SerializeField]
    private List<ForumPost> forumPosts;
    
    [Header("交互物PC，手动拖拽")]
    [SerializeField]
    private GameObject PC;

    void Start()
    {
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
    }

    private void OpenPost(int postIndex)
    {
        ForumPost post = forumPosts[postIndex];
        
        if (post.postUI != null)
        {
            post.postUI.SetActive(true);
        }
        
        Debug.Log($"打开帖子 {postIndex}");
    }

    private void ClosePost(int postIndex)
    {
        ForumPost post = forumPosts[postIndex];
        
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
        
        // 关闭论坛界面
        gameObject.SetActive(false);
        
        // 调用 PC 的关闭方法
        if (PC != null)
        {
            PC.GetComponent<I_PC>().ClosePC();
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
}