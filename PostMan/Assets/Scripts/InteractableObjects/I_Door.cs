using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEditor.PackageManager;
using PostMan.AudioSystem;

public class I_Door : MonoBehaviour, IInteractable
{
    [Header("动画组件")]
    [Tooltip("门把手动画组件在门把手预制体上，不在门把手预制体的父物体上")]
    [SerializeField] private Animator handleAnimator;
    [Tooltip("挂载该脚本的门对象上")]
    [SerializeField] private Animator doorAnimator;

    [Header("音效")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip stuckSound;
    
    [Header("门初始设置")]
    [Tooltip("门绕门轴逆时针打开")]
    [SerializeField] private bool openedSide1 = false;
    [Tooltip("门绕门轴顺时针打开")]
    [SerializeField] private bool openedSide2 = false;
    [Tooltip("门是否卡住")]
    public bool stucked = false;

    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }


    void Start()
    {

        doorAnimator.SetBool("Opened_Side_1" , openedSide1);
        doorAnimator.SetBool("Opened_Side_2" , openedSide2);

        
        if(doorAnimator.GetBool("Opened_Side_1") == true)
        {
            OpenDoor1();
        }

        if(doorAnimator.GetBool("Opened_Side_2") == true)
        {
            OpenDoor2();
        }
        
    }

    void Update()
    {
        doorAnimator.SetBool("Stucked" , stucked);
    }

    public void InteractWith(PlayerInteractor player)
    {

        //触发对话（未完成，等待对话系统接口）


        Debug.LogFormat("[I_Door.cs] 与门交互");

        Transform PlayerTF = player.gameObject.transform;

        if(doorAnimator.GetBool("Opened_Side_1") == false 
            && doorAnimator.GetBool("Stucked") == false
            && DetectPlayerSide(PlayerTF) < 0)
        {
            StartCoroutine(OpenDoor1());
        }

        else if(doorAnimator.GetBool("Opened_Side_1") == true )
            //&& DetectPlayerSide(PlayerTF) < 0)
        {
            StartCoroutine(CloseDoor1());
        }

        else if(doorAnimator.GetBool("Stucked") == true 
            && DetectPlayerSide(PlayerTF) < 0)
        {
            StartCoroutine(Stuck1());
        }

        else if(doorAnimator.GetBool("Opened_Side_2") == false 
            && doorAnimator.GetBool("Stucked") == false
            && DetectPlayerSide(PlayerTF) > 0)
        {
            StartCoroutine(OpenDoor2());
        }

        else if(doorAnimator.GetBool("Opened_Side_2") == true)
            //&& DetectPlayerSide(PlayerTF) > 0)
        {
            StartCoroutine(CloseDoor2());
        }

        else if(doorAnimator.GetBool("Stucked") == true 
            && DetectPlayerSide(PlayerTF) > 0)
        {
            StartCoroutine(Stuck2());
        }

        StartCoroutine(Relay());
    }

    /// <summary>
    /// 门绕门轴逆时针打开
    /// </summary>
    IEnumerator OpenDoor1()
    {
        doorAnimator.SetTrigger("Open_Side_1");
        handleAnimator.SetTrigger("Open");
        if(openSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , openSound , false , false , 0f , 1f);
        }

        yield return new WaitForSeconds(0.3f);
        doorAnimator.SetBool("Opened_Side_1" , true);

        doorAnimator.ResetTrigger("Open_Side_1");
        handleAnimator.ResetTrigger("Open");
    }

    /// <summary>
    /// 门绕门轴顺时针关闭
    /// </summary>
    IEnumerator CloseDoor1()
    {
        doorAnimator.SetTrigger("Close_Side_1");
        if(closeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , closeSound , false , false , 0f , 1f);
        }

        yield return new WaitForSeconds(0.4f);
        doorAnimator.SetBool("Opened_Side_1" , false);

        doorAnimator.ResetTrigger("Close_Side_1");
    }

    /// <summary>
    /// 门绕门轴逆时针打开时卡住 
    /// </summary>
    IEnumerator Stuck1()
    {
        doorAnimator.SetTrigger("Stuck_Side_1");
        handleAnimator.SetTrigger("Stuck");
        if(stuckSound)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , stuckSound , false , false , 0f , 1f);
        }

        yield return new WaitForSeconds(0.3f);

        doorAnimator.ResetTrigger("Stuck_Side_1");
        handleAnimator.ResetTrigger("Stuck");

    }

    /// <summary>
    /// 门绕门轴顺时针打开
    /// </summary>
    IEnumerator OpenDoor2()
    {
        doorAnimator.SetTrigger("Open_Side_2");
        handleAnimator.SetTrigger("Open");
        if(openSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , openSound , false , false , 0f , 1f);
        }

        yield return new WaitForSeconds(0.3f);
        doorAnimator.SetBool("Opened_Side_2" , true);

        doorAnimator.ResetTrigger("Open_Side_2");
        handleAnimator.ResetTrigger("Open");
    }

    /// <summary>
    /// 门绕门轴逆时针关闭
    /// </summary>
    IEnumerator CloseDoor2()
    {
        doorAnimator.SetTrigger("Close_Side_2");
        if(closeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , closeSound , false , false , 0f , 1f);
        }

        yield return new WaitForSeconds(0.3f);
        doorAnimator.SetBool("Opened_Side_2" , false);

        doorAnimator.ResetTrigger("Close_Side_2");
    }

    /// <summary>
    /// 门绕门轴顺时针打开时卡住
    /// </summary>
    IEnumerator Stuck2()
    {
        doorAnimator.SetTrigger("Stuck_Side_2");
        handleAnimator.SetTrigger("Stuck");
        if(stuckSound)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , stuckSound , false , false , 0f , 1f);
        }

        yield return new WaitForSeconds(0.3f);

        doorAnimator.ResetTrigger("Stuck_Side_2");
        handleAnimator.ResetTrigger("Stuck");
    }

    /// <summary>
    /// 检测玩家在门在哪一边
    /// </summary>
    /// <param name="Player"></param>
    /// <returns></returns>
    private float DetectPlayerSide(Transform Player)
    {
        //获取玩家到门的向量
        Vector3 PtoD = gameObject.transform.position - Player.position;

        //玩家到门的向量 和 门指向前方的向量 进行点乘
        return Vector3.Dot(PtoD , gameObject.transform.forward);
    }

    /// <summary>
    /// 暂时关闭交互，避免在动画播放时交互导致出错
    /// </summary>
    /// <returns></returns>
    IEnumerator Relay()
    {
        yield return new WaitForSeconds(0.3f);
        
        CanInteract = false;

        yield return new WaitForSeconds(1.2f);

        CanInteract = true;
    }

}
