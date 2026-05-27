using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务过渡配置：记录当前任务索引 与 完成后要开启的下一个任务的SO文件
/// 开发中发现完成当前任务并开启下一个任务时
/// 需要当前正在进行的任务的索引，和下一个任务的SO文件
/// 索性开一个类来记录这两个信息，在其他脚本可以用这个类创建List
/// </summary>
[Serializable]
public class TaskTransition
{
    public int currentIndex;

    public TaskSO nextTask;
}
