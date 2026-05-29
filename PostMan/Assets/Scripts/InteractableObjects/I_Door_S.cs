using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using Cinemachine;
using UnityEngine.Rendering.Universal;

public class I_Door_S : MonoBehaviour, IInteractable
{
    [Header("音效")]
    [Tooltip("突脸音效")]
    [SerializeField] private AudioClip FrightSound;
    private AudioSource audioSource;

    private Transform SquatCamera; //用于蹲下动作挂载的虚拟相机
    private Transform PlayerCamera; //角色正常移动时的虚拟相机
    private PlayerMotion playerMotion;  //角色移动组件
    private ShowInteractPrompt showInteractPrompt;  //显示交互提示组件
    private Animator cameraAnimator;  //虚拟相机(SquatCamera)上的动画组件
    private Animator doorAnimator;  //宠物门上的动画组件
    private Coroutine currentCoroutine;

    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    void Start()
    {

        //获取睡觉视角的虚拟相机
        SquatCamera = gameObject.transform.FindChildByName("SquatCamera");
        if(SquatCamera == null)
        {
            Debug.LogError("[I_Door_S.cs] 获取床虚拟相机失败");
        }

        //获取显示交互提示组件
        showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
        if(showInteractPrompt == null)
        {
            Debug.LogError("[I_Door_S.cs] 获取显示交互提示组件失败");
        }

        //获取动画组件
        cameraAnimator = gameObject.transform.FindChildByName("SquatCamera").GetComponent<Animator>();
        doorAnimator = gameObject.transform.FindChildByName("I_D_SD").GetComponent<Animator>();
        if(cameraAnimator == null || doorAnimator == null)
        {
            Debug.LogError("[I_Door_S.cs] 获取动画组件失败");
        }

        //获取音效组件
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            Debug.LogError("[I_Door_S.cs] 获取音效组件失败");
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Door_S.cs] 与宠物门交互");

        showInteractPrompt.CanSelect = false;

        //获取玩家正常移动视角的虚拟相机
        PlayerCamera = player.gameObject.transform.FindChildByName("FPVcam");

        //获取角色移动组件
        playerMotion = player.gameObject.GetComponent<PlayerMotion>();

        //暂时禁用角色移动组件
        playerMotion.enabled = false;

        //激活蹲下视角的虚拟相机
        SquatCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

        //失活玩家正常移动视角的虚拟相机，让视角自动过渡到蹲下视角
        PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(SquatDown());
    }

    private IEnumerator SquatDown()
    {
        yield return new WaitForSeconds(1.5f);

        cameraAnimator.SetTrigger("Down");

        yield return new WaitForSeconds(2f);

        cameraAnimator.ResetTrigger("Down");
        doorAnimator.SetTrigger("Open");

        //Day4黑手惊吓（未完成）

        //突脸音效
        if(FrightSound != null)
        {
            audioSource.PlayOneShot(FrightSound);
        }

        yield return new WaitForSeconds(3f);

        doorAnimator.ResetTrigger("Open");
        doorAnimator.SetTrigger("Close");

        yield return new WaitForSeconds(3f);

        doorAnimator.ResetTrigger("Close");
        cameraAnimator.SetTrigger("Up");

        yield return new WaitForSeconds(1f);

        cameraAnimator.ResetTrigger("Up");

        //重新启用允许交互
        showInteractPrompt.CanSelect = true;

        //激活玩家正常移动视角的虚拟相机
        PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

        //失活蹲下视角的虚拟相机，让视角自动过渡到玩家正常视角
        SquatCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

        //启用角色移动组件
        playerMotion.enabled = true;

    }
}
