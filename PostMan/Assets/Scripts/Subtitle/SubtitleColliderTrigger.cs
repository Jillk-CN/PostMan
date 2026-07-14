using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleColliderTrigger : MonoBehaviour
{
    public string subtitleKey;
    [SerializeField]
    private bool TriggerOnce = false;

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
    }
}
