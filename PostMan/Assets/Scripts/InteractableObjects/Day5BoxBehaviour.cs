using System.Collections;
using System.Collections.Generic;
using PostMan.AudioSystem;
using PostMan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day5BoxBehaviour : MonoBehaviour
{
    public Button extraButton;
    public TextMeshProUGUI content;
    public Sprite Side;
    public string subtitleKey;
    public GameObject mailBox404;
    private I_MailBox i_MailBox;

    void Start()
    {   
        i_MailBox = mailBox404.GetComponent<I_MailBox>();

        extraButton.gameObject.SetActive(true);

        extraButton.onClick.AddListener(CheckSide);

        content.text = "查看侧面";
    }

    private void CheckSide()
    {
        
        ViewImagePanel.Instance.ShowImage(Side , true);

        SubtitleUI.Instance.TypeSubtitle(subtitleKey , 4f);

        extraButton.onClick.RemoveListener(CheckSide);

        extraButton.gameObject.SetActive(false);
    }
}
