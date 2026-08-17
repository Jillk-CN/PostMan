using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using Cinemachine;
using PostMan.UI;
using PostMan.InputManagement;

public class I_PC : MonoBehaviour, IInteractable
{
    [Header("论坛界面（手动拖拽论坛UI）")]
    [SerializeField]private GameObject forum;
    private Transform PCCamera; //PC挂载的虚拟相机
    private Transform PlayerCamera; //角色正常移动时的虚拟相机
    private PlayerMotion playerMotion;  //角色移动组件
    private ShowInteractPrompt showInteractPrompt;  //显示交互提示组件

    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    private GameInputManager gameInputManager;

    void Start()
    {
        //获取PC视角的虚拟相机
        PCCamera = gameObject.transform.FindChildByName("PCCamera");
        if(PCCamera == null)
        {
            Debug.LogError("[I_PC.cs] 获取PC虚拟相机失败");
        }

        //获取显示交互提示组件
        showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
        if(showInteractPrompt == null)
        {
            Debug.LogError("[I_PC.cs] 获取显示交互提示组件失败");
        }

        gameInputManager = FindAnyObjectByType<GameInputManager>();
    }
    public void InteractWith(PlayerInteractor player)
    {
        if(CanInteract == false) return;

        Debug.LogFormat("[I_PC.cs] 与电脑交互");

        //获取玩家正常移动视角的虚拟相机
        PlayerCamera = player.gameObject.transform.FindChildByName("FPVcam");

        //获取角色移动组件
        playerMotion = player.gameObject.GetComponent<PlayerMotion>();

        if (gameInputManager != null)
        {
            gameInputManager.SetPlayerAllInput(false);
        }

        //激活PC视角的虚拟相机
        PCCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

        //失活玩家正常移动视角的虚拟相机，让视角自动过渡到睡觉视角
        PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        BlackScreen.Instance.BlackIn("" , 0.5f);

        StartCoroutine(OpenForum());
    }

    public void ClosePC()
    {
        StartCoroutine(EscapePC());
    }

    private IEnumerator EscapePC()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        BlackScreen.Instance.BlackOut("" , 0.5f);

        //退出论坛界面（用scale隐藏而非SetActive，避免中断协程）
        forum.transform.localScale = Vector3.zero;

        //激活玩家正常移动视角的虚拟相机
        PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

        //失活PC视角的虚拟相机，让视角自动过渡到正常移动视角  
        PCCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

        //立即恢复角色移动和交互（无需等待黑屏过渡完成）
        if (gameInputManager != null)
        {
            gameInputManager.SetPlayerAllInput(true);
        }

        yield return null;
    }

    private IEnumerator OpenForum()
    {
        yield return new WaitForSeconds(0.5f);

        //进入论坛界面（用scale显示）
        forum.transform.localScale = Vector3.one;
    }
}
