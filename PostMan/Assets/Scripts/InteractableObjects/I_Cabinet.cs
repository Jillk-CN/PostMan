using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using PostMan.AudioSystem;

public class I_Cabinet : MonoBehaviour, IInteractable
{
    [Header("是否为410货柜")]
    [SerializeField]private bool is410Cabinet = false;

    [Header("动画组件")]
    [Tooltip("挂载该脚本的门对象上")]
    [SerializeField] private Animator doorAnimator;

    [Header("音效")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip stuckSound;
    [SerializeField] private AudioClip openHeavySound;
    [SerializeField] private AudioClip closeHeavySound;
    [SerializeField] private AudioClip knockLightSound;
    [SerializeField] private AudioClip knockHeavySound;
    [SerializeField] private AudioClip insideScrapeSound;
    [SerializeField] private AudioClip metalVibrateSound;
    [SerializeField] private AudioClip cabinetDropSound;
    
    
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
            if (stuckSound != null)
            {
                AudioManager.Instance.Play(AudioTrackId.FX , stuckSound , false , false , 0f , 1f);
            }
            
            StartCoroutine(Stuck());
        }
        else if (!isOpen)
        {
            if (openSound != null && !is410Cabinet)
            {
                AudioManager.Instance.Play(AudioTrackId.FX , openSound , false , false , 0f , 1f);
            }
            else if(openSound != null && is410Cabinet)
            {
                AudioManager.Instance.Play(AudioTrackId.FX , openHeavySound , false , false , 0f , 1f);
            }

            StartCoroutine(OpenDoor());
        }
        else
        {
            if (closeSound != null && !is410Cabinet)
            {
                AudioManager.Instance.Play(AudioTrackId.FX , closeSound , false , false , 0f , 1f);
            }
            else if(closeSound != null && is410Cabinet)
            {
                AudioManager.Instance.Play(AudioTrackId.FX , closeHeavySound , false , false , 0f , 1f);
            }
            StartCoroutine(CloseDoor());
        }
    }

    /// <summary>
    /// 开门
    /// </summary>
    IEnumerator OpenDoor()
    {
        doorAnimator.SetTrigger("Open_Side_2");
        
        

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

    public void OpenHeavy()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , openHeavySound);
        StartCoroutine(OpenDoor());
    }

    public void CloseHeavy()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , closeHeavySound);
        StartCoroutine(CloseDoor());
    }

    public void KnockLight()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , knockLightSound);
    }

    public void KnockHeavy()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , knockHeavySound);
    }

    public void InsideScrape()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , insideScrapeSound);
    }

    public void MetalVibrate()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , metalVibrateSound);
        StartCoroutine(Stuck());
    }

    public void CabinetDrop()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , cabinetDropSound);
    }
}
