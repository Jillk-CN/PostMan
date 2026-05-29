using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;

public class I_Message : MonoBehaviour, IInteractable
{
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Message.cs] 与前台便签(Message)交互");

        //进入阅读系统（未完成，等待阅读系统接口）
        //获取当天任务信息
    }
}
