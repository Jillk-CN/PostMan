using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class I_Piece : MonoBehaviour, IInteractable
{
    [Header("任务提示字幕索引（只有一条字幕两个就填一样）")]
    [SerializeField] private SubtitlePart subtitlePart;

    [Header("要推进的任务索引")]
    [SerializeField] private int taskIndex;

    [Header("任务/剧情推进量（不推进就填0）")]
    [SerializeField] private int advanceValue;
    
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Piece.cs] 与纸条 / 回执交互");

        //阅读系统接口

        SubtitleUI.Instance.TypeSubtitle(subtitlePart.startIndex , subtitlePart.endIndex);

        TaskManager.Instance.AdvanceTask(taskIndex , advanceValue);
    }
}
