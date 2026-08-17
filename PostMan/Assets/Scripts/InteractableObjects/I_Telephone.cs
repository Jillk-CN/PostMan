using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using PostMan.AudioSystem;

public class I_Telephone : MonoBehaviour
{
    [Header("音效")]
    [SerializeField]private AudioClip phoneSound;
    [SerializeField]private AudioClip abnormalPhoneSound;
    [SerializeField]private AudioClip pickUp;
    [SerializeField]private AudioClip putDown;
    [SerializeField]private AudioClip handUp;

    //话筒
    private Transform handle;

    /*暂时没有字幕的功能,先隐藏掉
    [Header("字幕片段Key列表")]
    [SerializeField] private List<string> subtitleKeyList;
    [Header("延迟设置（设置为0，则字幕播放完毕后就放回话筒")]
    [SerializeField] private float delayTime = 0;
     */
    /// <summary>
    /// 玩家的sightPoint子物体,名字要和变量名一样
    /// </summary>
    private Transform sightPoint;
    [Tooltip("玩家拿着电话的时候的位置")]
    [SerializeField]
    private Vector3 holdingPosition = new Vector3(0.148f, 0.005f, 0.341f);
    [SerializeField]
    [Tooltip("玩家拿着电话的时候的旋转角度")]
    private Vector3 holdingEuler = new Vector3(-233.1f, -1.809f, -69.83f);
    private Vector3 defaultPosition;
    private Vector3 defaultEuler;
    private bool holding;

    private void Start()
    {
        /*
        //获取显示交互提示组件
        showInteractPrompt = gameObject.GetComponent<ShowInteractPrompt>();
        if(showInteractPrompt == null)
        {
            Debug.LogError("[I_Telephone.cs] 获取显示交互提示组件失败");
        }
         */

        handle = this.transform.FindChildByName(nameof(handle));
        sightPoint = PlayerInstance.Instance.transform.FindChildByName(nameof(sightPoint));
        defaultPosition = handle.transform.localPosition;
        defaultEuler = handle.transform.localEulerAngles;
    }
    public void PickUp()
    {
        AudioManager.Instance.Stop(AudioTrackId.FX, fadeOut: true, fadeOutDuration: 0f);
        AudioManager.Instance.Play(AudioTrackId.FX , pickUp);

        handle.transform.parent = sightPoint;
        handle.transform.localPosition = holdingPosition;
        handle.transform.localEulerAngles = holdingEuler;
        holding = true;

    }
    public void PutDown()
    {
        AudioManager.Instance.Play(AudioTrackId.FX , putDown);

        handle.transform.parent = this.transform;
        handle.transform.localPosition = defaultPosition;
        handle.transform.localEulerAngles = defaultEuler;
        holding = false;
    }
    /// <summary>
    /// 玩家是否拿着电话
    /// </summary>
    /// <returns></returns>
    public bool Holding() 
    {
        return holding;
    } 

    /*
    private IEnumerator MoveHandle()
    {
        float elapsed = 0;
        while (elapsed<moveDuration)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }
    }
     */
    /*旧的实现
     
    private IEnumerator Call()
    {
        AudioManager.Instance.Stop(AudioTrackId.FX, fadeOut: true, fadeOutDuration: 0f);

        AudioManager.Instance.Play(AudioTrackId.FX , pickUp);

        //AudioManager.Instance.Play(AudioTrackId.FX , phoneStatic , true);

        playerMotion.enabled = false;

        showInteractPrompt.CanSelect = false;

        handle.transform.parent = sightTrans;

        handle.transform.localPosition = new Vector3(0.148f , 0.005f , 0.341f);

        handle.transform.localRotation = Quaternion.Euler(-233.1f , -1.809f , -69.83f);

        yield return StartCoroutine(OrderPlaySubtitle());

        //AudioManager.Instance.Stop(AudioTrackId.FX, fadeOut: true, fadeOutDuration: 0f);

        yield return new WaitForSeconds(delayTime);

        AudioManager.Instance.Play(AudioTrackId.FX , handUp);

        handle.transform.parent = gameObject.transform;

        handle.transform.localPosition = new Vector3(0.0089f , 0.028f , 0.0238f);

        handle.transform.localRotation = Quaternion.Euler(0 ,0 ,0);
        
        playerMotion.enabled = true;

        showInteractPrompt.CanSelect = true;
    }

     */

    /*旧的实现
    private IEnumerator OrderPlaySubtitle()
    {

        //等待天数条件判断播放哪段字幕，先默认索引为0
        if(subtitleKeyList != null && subtitleKeyList.Count > 0)
        {
            SubtitleUI.Instance.TypeSubtitle(subtitleKeyList[0]);
        }
        else
        {
            Debug.LogWarning("[I_Telephone] subtitleKeyList 为空，无法播放字幕");
        }

        while(SubtitleUI.Instance.isTyping)
        {
            yield return null;
        }
    }

     */
}
