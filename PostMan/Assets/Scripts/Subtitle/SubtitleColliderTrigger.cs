using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Scene;
using UnityEngine;

public class SubtitleColliderTrigger : MonoBehaviour
{
    public int sceneOrder = -1;
    public string subtitleKey;
    public AudioClip clip;
    [SerializeField]
    private bool TriggerOnce = false;
    [SerializeField]
    private bool PlayBGM = false;

    private bool haveTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if(sceneOrder != -1 && sceneOrder != SceneInitializer.Instance.SceneOrder)
        {
            Debug.LogWarning($"[SubtitleColliderTrigger] sceneOrder({sceneOrder}) 与当前场景次序({SceneInitializer.Instance.SceneOrder})不匹配，已跳过");
            return;
        }
        
        if(TriggerOnce == true && haveTrigger == false)
        {
            SubtitleUI.Instance.TypeSubtitle(subtitleKey);

            haveTrigger = true;

            return;
        }
        else if(TriggerOnce == true && haveTrigger == true)
        {
            return;
        }
        else
        {
            SubtitleUI.Instance.TypeSubtitle(subtitleKey);
        }

        if (PlayBGM)
        {
            AudioManager.Instance.Stop(AudioTrackId.BGM);
            AudioManager.Instance.Play( AudioTrackId.BGM, clip, true, true);
        }
    }
}
