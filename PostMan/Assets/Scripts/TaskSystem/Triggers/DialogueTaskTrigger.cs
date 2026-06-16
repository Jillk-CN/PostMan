using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于对话系统开始任务
public class DialogueTaskTrigger : BaseTaskTrigger
{
    protected override bool ShouldTrigger()
    {
        return true;
    }
    public void TriggerTask()
    {
        base.TryTrigger();
    }
}
