using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemState
{
    /// <summary>
    /// 对应交互物的GameObject
    /// </summary>
    public GameObject gameObject;

    /// <summary>
    /// 对应交互物的uniqueID
    /// </summary>
    [Header("对应交互物的uniqueID, 即交互物的ObjectId组件值")]
    public string uniqueID;

    /// <summary>
    /// 该交互物是否可交互
    /// </summary>
    [Header("是否可交互")]
    public bool isCanInteract = false;

    /// <summary>
    /// 该交互物是否可视
    /// </summary>
    [Header("是否可视")]
    public bool isVisuable = false;
}
