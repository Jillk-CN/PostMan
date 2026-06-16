using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.AudioSystem;

public class I_Piece : MonoBehaviour, IInteractable
{
    public AudioClip takeSound;
    [Header("任务提示字幕索引（只有一条字幕两个就填一样）")]
    [SerializeField] private string subtitleKey;

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

        if(takeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , takeSound);
        }

        SubtitleUI.Instance.TypeSubtitle(subtitleKey);

        TaskManager.Instance.AdvanceTask(taskIndex , advanceValue);
    }
}
