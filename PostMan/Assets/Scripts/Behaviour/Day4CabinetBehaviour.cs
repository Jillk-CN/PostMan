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

    private bool hasStarted = false;

    void OnEnable()
    {
        GameSceneManager.OnSceneSwitchCompleted += OnSceneSwitchCompleted;
        TryStart();
    }

    void OnDisable()
    {
        GameSceneManager.OnSceneSwitchCompleted -= OnSceneSwitchCompleted;
    }

    private void OnSceneSwitchCompleted(IReadOnlyList<string> loadedScenes)
    {
        TryStart();
    }

    private void TryStart()
    {
        if(hasStarted) return;

        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        if(i_Cabinet == null)
        {
            i_Cabinet = GetComponent<I_Cabinet>();
        }

        if(i_Cabinet == null)
        {
            Debug.LogWarning("[Day4CabinetBehaviour] 未找到 I_Cabinet 组件，无法订阅关门事件");
            return;
        }

        i_Cabinet.OnDoorClose += OnDoorClose;

        hasStarted = true;
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
