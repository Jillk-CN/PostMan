using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine.XR;

public class I_Telephone : MonoBehaviour, IInteractable
{
    [Header("音效")]
    [SerializeField]private AudioClip phoneSound;
    private AudioSource audioSource;

    [Header("话筒对象")]
    [SerializeField] private GameObject handle;

    [Header("字幕片段Key列表")]
    [SerializeField] private List<string> subtitleKeyList;
    [Header("延迟设置（设置为0，则字幕播放完毕后就放回话筒")]
    [SerializeField] private float delayTime = 0;

    private PlayerMotion playerMotion;  //角色移动组件
    private ShowInteractPrompt showInteractPrompt;  //显示交互提示组件
    private Transform sightTrans;
    private Vector3 defaultPosi;
    private Quaternion defaultRota;


    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }

    void Start()
    {
        //获取音效组件
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            Debug.LogError("[I_Telephone.cs] 获取音效组件失败");
        }

        //获取显示交互提示组件
        showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
        if(showInteractPrompt == null)
        {
            Debug.LogError("[I_Telephone.cs] 获取显示交互提示组件失败");
        }

        handle = gameObject.transform.FindChildByName("I_P_Handle").gameObject;
    }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Telephone.cs] 与电话交互");

        //获取角色移动组件
        playerMotion = player.gameObject.GetComponent<PlayerMotion>();
        sightTrans = player.gameObject.transform.FindChildByName("sightPoint").transform;

        StartCoroutine(Call());
    }

    private IEnumerator Call()
    {
        playerMotion.enabled = false;

        showInteractPrompt.CanSelect = false;

        handle.transform.parent = sightTrans;

        handle.transform.localPosition = new Vector3(0.148f , 0.005f , 0.341f);

        handle.transform.localRotation = Quaternion.Euler(-233.1f , -1.809f , -69.83f);

        if(phoneSound != null)
        {
            audioSource.PlayOneShot(phoneSound);
        }

        OrderPlaySubtitle();

        yield return new WaitForSeconds(delayTime);

        handle.transform.parent = gameObject.transform;

        handle.transform.localPosition = new Vector3(0.0089f , 0.028f , 0.0238f);

        handle.transform.localRotation = Quaternion.Euler(0 ,0 ,0);
        
        playerMotion.enabled = true;

        showInteractPrompt.CanSelect = true;
    }

    private void OrderPlaySubtitle()
    {
        //等待天数条件判断播放哪段字幕，先默认索引为0
        if(true)
        {
            SubtitleUI.Instance.TypeSubtitle(subtitleKeyList[0]);
        }
    }
}
