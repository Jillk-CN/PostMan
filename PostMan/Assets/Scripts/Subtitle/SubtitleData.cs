using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class SubtitleData : ScriptableObject
{
    [Header("字幕表（需按字幕ID顺序输入）")]
    public List<SubtitleContent> subtitleList;
}

[Serializable]
public class SubtitleContent
{
    [Tooltip("字幕ID")]
    public int ID;
    [Tooltip("字幕内容")]
    public string ContentCN;
    public string ContentEN;
    [Tooltip("自动播放延迟时间（没有延迟时间设置为0即可）")]
    public float DelayTime;
}

