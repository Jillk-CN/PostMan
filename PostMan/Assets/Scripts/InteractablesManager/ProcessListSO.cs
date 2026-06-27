using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interactables Process List")]
public class InteractablesProcessListSO : ScriptableObject
{
    [Header("清单中的状态值为目标值\n清单中GameObject一栏置空，只需设置其他三项")]
    public List<ItemState> item;
}
