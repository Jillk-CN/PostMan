using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Scene;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class ColliderSFXSwitcher : MonoBehaviour
{
    [Header("生效的SceneOrder")]
    public int sceneOrder = 0;
    [Header("SFX")]
    public AudioClip sfx;
    [Header("延迟时长")]
    public float delayTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;
         
        StartCoroutine(Run());
    }
    
    private IEnumerator Run()
    {
        yield return new WaitForSeconds(delayTime);
        AudioManager.Instance.Play(AudioTrackId.FX , sfx);
    }
}
