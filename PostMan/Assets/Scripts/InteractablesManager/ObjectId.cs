using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于在交互物全局管理器建立缓存时识别并获取交互物
/// 挂载到需要管理的交互物上
/// objectId是唯一的
/// </summary>
public class ObjectId : MonoBehaviour
{
    [Header("挂载到需要管理的交互物上(即要被设置为是否隐藏的物体上)\n场景加载后若Unique Id未填写默认为挂载对象名称\n!!!需要保证场景中不存在相同的Unique Id!!!\n强烈建议命名为 UniqueID + 所属场景名")]
    public string uniqueID = "";
    void Awake()
    {
        if(uniqueID == "")
        {
            uniqueID = gameObject.name;
        }
    }
}