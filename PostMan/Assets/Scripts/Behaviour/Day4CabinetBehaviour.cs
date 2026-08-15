using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.Player;
using PostMan.Scene;
using UnityEngine;

public class Day4CabinetBehaviour : MonoBehaviour
{
    [Header("SubtitleKey")]
    public string subtitleKey;
    [Header("SFX")]
    public AudioClip knockSFX;

    [Header("启用的SceneOrder")]
    public int sceneOrder;
    private I_Cabinet i_Cabinet;

    void Start()
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        i_Cabinet.OnDoorClose += OnDoorClose;
    }

    private IEnumerator Knock()
    {
        yield return new WaitForSeconds(1f);

        GetComponent<I_Cabinet>().stucked = true;

        yield return new WaitForSeconds(1.5f);

        AudioManager.Instance.Play(AudioTrackId.FX , knockSFX);

        yield return new WaitForSeconds(2f);
        SubtitleUI.Instance.TypeSubtitle(subtitleKey);

        i_Cabinet.OnDoorClose -= OnDoorClose;
    }

    private void OnDoorClose()
    {
        StartCoroutine(Knock());
    }
}
