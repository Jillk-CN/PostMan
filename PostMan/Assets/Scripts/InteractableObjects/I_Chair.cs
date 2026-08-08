using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using Cinemachine;
using PostMan.Dialogue;
using System.Linq;

public class I_Chair : MonoBehaviour, IInteractable
{
    [Header("音效")]
    [Tooltip("坐下音效")]
    [SerializeField] private AudioClip sitDownSound;

    private Transform SitCamera; //用于坐下动作挂载的虚拟相机
    private Transform PlayerCamera; //角色正常移动时的虚拟相机
    private PlayerMotion playerMotion;  //角色移动组件
    private ShowInteractPrompt showInteractPrompt;  //显示交互提示组件
    private Animator animator;  //虚拟相机(SitCamera)上的动画组件
    private Coroutine currentCoroutine;

    [SerializeField]
    private DialogueSequence dialogue;
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    void Start()
    {

        //获取睡觉视角的虚拟相机
        SitCamera = gameObject.transform.FindChildByName("SitCamera");
        if(SitCamera == null)
        {
            Debug.LogError("[I_Chair.cs] 获取床虚拟相机失败");
        }

        //获取显示交互提示组件
        showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
        if(showInteractPrompt == null)
        {
            Debug.LogError("[I_Chair.cs] 获取显示交互提示组件失败");
        }

        //获取动画组件
        animator = gameObject.transform.FindChildByName("SitCamera").GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("[I_Chair.cs] 获取动画组件失败");
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        if(CanInteract == false) return;
        showInteractPrompt.CanSelect = false;

        //获取玩家正常移动视角的虚拟相机
        PlayerCamera = player.gameObject.transform.FindChildByName("FPVcam");

        //获取角色移动组件
        playerMotion = player.gameObject.GetComponent<PlayerMotion>();

        //暂时禁用角色移动组件
        playerMotion.enabled = false;

        //激活坐下视角的虚拟相机
        SitCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = true;

        //失活玩家正常移动视角的虚拟相机，让视角自动过渡到坐下视角
        PlayerCamera.gameObject.GetComponent<CinemachineVirtualCamera>().enabled = false;

        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(Sit());

    }

    private IEnumerator Sit()
    {
        yield return new WaitForSeconds(1.4f);

        animator.SetTrigger("Sit");

        yield return new WaitForSeconds(2f);

        animator.ResetTrigger("Sit");

        //最终对话（未完成，等待对话系统接口）
        DialogueManager.Instance.StartDialogue(dialogue,
            this.GetComponents<IPerformDataProvider>().ToList());
    }

}
