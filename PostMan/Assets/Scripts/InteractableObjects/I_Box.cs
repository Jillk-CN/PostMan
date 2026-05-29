using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;

public class I_Box : MonoBehaviour , IInteractable
{
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Box.cs] 与箱子交互");

        Transform child = gameObject.transform.GetChild(0);
        if(child != null)
        {
            child.gameObject.GetComponent<Renderer>().enabled = false;
        }

        StartCoroutine(DelayDestory());
    }

    //延迟销毁，避免交互系统访问被销毁的组件导致报错
    private IEnumerator DelayDestory()
    {
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
