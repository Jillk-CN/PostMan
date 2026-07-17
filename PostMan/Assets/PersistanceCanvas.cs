using UnityEngine;
using PostMan.UI;
using System;

public class SettingPanelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private GameObject pausePanel;

    private void Start()
    {
        // 在 Start 中订阅，确保所有 Awake/Init 已完成，TitleUIManager.Instance 必然不为 null
        TitleUIManager.Instance.OpenSettingsEvent.AddListener(OnOpenSettingPanel);
        TitleUIManager.Instance.CloseSettingsEvent.AddListener(OnCloseSettingPanel);
        PausePanel.Instance.ReturnToTitleEvent.AddListener(OnReturnToTitle);
    }

    private void OnReturnToTitle()
    {
        pausePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (TitleUIManager.Instance != null)
        {
            TitleUIManager.Instance.OpenSettingsEvent.RemoveListener(OnOpenSettingPanel);
            TitleUIManager.Instance.CloseSettingsEvent.RemoveListener(OnCloseSettingPanel);
        }
    }

    private void OnOpenSettingPanel()
    {
        settingPanel.SetActive(true);
        // Handle the event when the setting panel is opened
    }

    private void OnCloseSettingPanel()
    {
        settingPanel.SetActive(false);
        // Handle the event when the setting panel is closed
    }
}