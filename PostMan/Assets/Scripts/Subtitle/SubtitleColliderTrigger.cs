using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using UnityEngine;

public class SubtitleColliderTrigger : MonoBehaviour
{
    public string subtitleKey;
    public AudioClip clip;
    [SerializeField]
    private bool TriggerOnce = false;
    [SerializeField]
    private bool PlayBGM = false;

    private bool haveTrigger = false;

    void OnTriggerEnter(Collider other)
    {
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
