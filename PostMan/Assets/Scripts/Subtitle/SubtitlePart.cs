using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于记录一个字幕片段的起始索引和结束索引
/// 可被用于需要重复使用去播放多段不同字幕的脚本，如用List<SubtitlePart>
/// 若只播放一句字幕，则起始结束索引都填相同
/// 可被序列化
/// </summary>
[Serializable]
public class SubtitlePart
{
    public int startIndex;
    public int endIndex;
}
