using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using PostMan.AudioSystem;

public class I_Door_S : MonoBehaviour, IInteractable
{
    [Header("当前410门需要执行的动作\n（执行后自动复位，一次交互只能执行一个行为，单选）")]
    [Tooltip("放下包裹，黑手拖包裹")]
    public bool _PlaceBox = false;
    [Tooltip("轻敲门，门后回应")]
    public bool _Knock = false;
    [Tooltip("蹲下查看宠物门")]
    public bool _CheckDoor = true;

    [Header("交互物管理清单SO")]
    [SerializeField]private InteractablesProcessListSO Task18listSO;

    [Header("音效")]
    [SerializeField] private AudioClip placeGroundSound;
    [SerializeField] private AudioClip dragSlowSound;
    [SerializeField] private AudioClip doorCreakSound;
    [SerializeField] private AudioClip door410KnockSound;
    [SerializeField] private AudioClip door410KnockBackSound;

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
    }

    public void InteractWith(PlayerInteractor player)
    {
        if(CanInteract == false) return;

        if(_PlaceBox && !_CheckDoor && !_Knock)
        {
            StartCoroutine(PlaceBox());

            _PlaceBox = false;

            return;
        }

        if(_Knock && !_CheckDoor && !_PlaceBox)
        {
            StartCoroutine(KnockDoor());

            _Knock = false;

            return;
        }

        ///summary
        /// 暂时
        return;
        ///
        
        if(_CheckDoor && !_Knock && !_PlaceBox)
        {
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

            _CheckDoor = true;

            return;
        }
    }

    private IEnumerator SquatDown()
    {
        yield return new WaitForSeconds(1.5f);

        cameraAnimator.SetTrigger("Down");

        yield return new WaitForSeconds(2f);

        cameraAnimator.ResetTrigger("Down");
        doorAnimator.SetTrigger("Open");

        AudioManager.Instance.Play(AudioTrackId.FX , doorCreakSound);

        //Day4黑手惊吓（未完成）

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

    /// <summary>
    /// 在410门前放置箱子
    /// </summary>
    private IEnumerator PlaceBox()
    {
        InteractableManager.Instance.ApplyState(Task18listSO);

        AudioManager.Instance.Play(AudioTrackId.FX , placeGroundSound);

        yield return new WaitForSeconds(3f);

        //黑手拖包裹动作

        AudioManager.Instance.Play(AudioTrackId.FX , dragSlowSound);
    }

    private IEnumerator KnockDoor()
    {
        //敲门

        AudioManager.Instance.Play(AudioTrackId.FX , door410KnockSound);

        yield return new WaitForSeconds(2f);

        AudioManager.Instance.Play(AudioTrackId.FX , door410KnockBackSound);
    }
}
