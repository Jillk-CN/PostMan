using System.Collections;
using System.Collections.Generic;
using PostMan.Scene;
using PostMan.AudioSystem;
using UnityEngine;
using System.ComponentModel.Design;

/// <summary>
/// 通过触发触发器来更新交互物状态
/// </summary>
public class IMColliderTrigger : MonoBehaviour
{
    [Header("延迟时长")]
    public float time = 0f;

    [Header("交互物处理清单SO")]
    public InteractablesProcessListSO interactablesProcessListSO;

    [Header("在哪个SceneOrder里生效")]
    public int sceneOrder;

    [Header("音效")]
    public AudioClip sfx;

    [Header("单次触发")]
    public bool triggerOnce = false;
    private bool haveTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if(triggerOnce && haveTrigger)return;

        if(SceneInitializer.Instance.SceneOrder != sceneOrder)return;

        StartCoroutine(ApplyList());

        if(triggerOnce)haveTrigger = true;
    }

    private IEnumerator ApplyList()
    {
        yield return new WaitForSeconds(time);

        if(sfx != null)AudioManager.Instance.Play(AudioTrackId.FX , sfx);

        InteractableManager.Instance.ApplyState(interactablesProcessListSO);
    }
}
