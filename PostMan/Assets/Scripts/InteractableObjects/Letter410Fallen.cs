using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using PostMan.InputManagement;
using UnityEngine;
using UnityEngine.UI;
using PostMan.UI;
using PostMan.AudioSystem;

public class Letter410Fallen : MonoBehaviour
{
    public ReadingContent content;
    public GameObject message;
    public GameObject photo;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button messageQuitButton;
    public string subtitleKey1;
    public string subtitleKey2;
    public string subtitleKey3;


    public AudioClip dropSFX;

    private void OnEnable()
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);

        button1.onClick.AddListener(CheckDestroy);
        button2.onClick.AddListener(PutBack);
        button3.onClick.AddListener(TypeSTL);
        messageQuitButton.onClick.AddListener(DropPhoto);

    }

    private void CheckDestroy()
    {
        ViewImagePanel.Instance.Hide();
        TextPanel.Instance.ShowText(content);

        SubtitleUI.Instance.TypeSubtitle(subtitleKey1);
        GameInputManager.Instance.ShowCursor();

        button1.onClick.RemoveListener(CheckDestroy);
        button2.onClick.RemoveListener(PutBack);

        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
    }

    private void PutBack()
    {
        message.SetActive(true);
        ViewImagePanel.Instance.Hide();

        SubtitleUI.Instance.TypeSubtitle(subtitleKey2);

        AudioManager.Instance.Play(AudioTrackId.FX , dropSFX);

        button1.onClick.RemoveListener(CheckDestroy);
        button2.onClick.RemoveListener(PutBack);

        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
    }

    private void DropPhoto()
    {
        photo.SetActive(true);

        AudioManager.Instance.Play(AudioTrackId.FX , dropSFX);

        messageQuitButton.onClick.RemoveListener(DropPhoto);
    }

    private void TypeSTL()
    {
        SubtitleUI.Instance.TypeSubtitle(subtitleKey3);

        button3.onClick.RemoveListener(TypeSTL);
    }
}
