using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;

public class I_Cabinet : MonoBehaviour, IInteractable
{
    [Header("动画组件")]
    [Tooltip("挂载该脚本的门对象上")]
    [SerializeField] private Animator doorAnimator;

    [Header("音效")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip stuckSound;
    private AudioSource audioSource;
    
    [Header("门初始设置")]
    [Tooltip("门是否卡住")]
    public bool stucked = false;

    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract = value; }
    public int Priority { get => priority; set => priority = value; }

    // 门的状态
    private bool isOpen = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError(gameObject.name + "获取音效组件失败");
        }

    }

    void Update()
    {
        doorAnimator.SetBool("Stucked", stucked);
    }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Cabinet.cs] 与410后仓柜子交互");

        // 根据门的状态和是否卡住执行对应操作
        if (doorAnimator.GetBool("Stucked"))
        {
            StartCoroutine(Stuck());
        }
        else if (!isOpen)
        {
            StartCoroutine(OpenDoor());
        }
        else
        {
            StartCoroutine(CloseDoor());
        }
    }

    /// <summary>
    /// 开门
    /// </summary>
    IEnumerator OpenDoor()
    {
        doorAnimator.SetTrigger("Open_Side_2");
        
        if (openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        yield return new WaitForSeconds(0.3f);
        
        doorAnimator.SetBool("Opened_Side_2", true);
        isOpen = true;

        doorAnimator.ResetTrigger("Open_Side_2");
        
        // 触发交互冷却
        StartCoroutine(Relay());
    }

    /// <summary>
    /// 关门
    /// </summary>
    IEnumerator CloseDoor()
    {
        doorAnimator.SetTrigger("Close_Side_2");
        
        if (closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        yield return new WaitForSeconds(0.4f);
        
        doorAnimator.SetBool("Opened_Side_2", false);
        isOpen = false;

        doorAnimator.ResetTrigger("Close_Side_2");
        
        // 触发交互冷却
        StartCoroutine(Relay());
    }

    /// <summary>
    /// 门卡住时的效果
    /// </summary>
    IEnumerator Stuck()
    {
        doorAnimator.SetTrigger("Stuck_Side_1");
        
        if (stuckSound != null)
        {
            audioSource.PlayOneShot(stuckSound);
        }

        yield return new WaitForSeconds(0.3f);

        doorAnimator.ResetTrigger("Stuck_Side_1");
        
        // 触发交互冷却
        StartCoroutine(Relay());
    }

    /// <summary>
    /// 暂时关闭交互，避免在动画播放时交互导致出错
    /// </summary>
    IEnumerator Relay()
    {
        canInteract = false;
        
        yield return new WaitForSeconds(1f);
        
        canInteract = true;
    }
}