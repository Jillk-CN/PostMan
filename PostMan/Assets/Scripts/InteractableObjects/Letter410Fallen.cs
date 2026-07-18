using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
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
    public Button messageQuitButton;

    public AudioClip dropSFX;

    private void OnEnable()
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);

        button1.onClick.AddListener(CheckDestroy);
        button2.onClick.AddListener(PutBack);
        messageQuitButton.onClick.AddListener(DropPhoto);

    }

    private void CheckDestroy()
    {
        ViewImagePanel.Instance.Hide();
        TextPanel.Instance.ShowText(content);

        button1.onClick.RemoveListener(CheckDestroy);
        button2.onClick.RemoveListener(PutBack);

        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
    }

    private void PutBack()
    {
        message.SetActive(true);
        ViewImagePanel.Instance.Hide();

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
}
