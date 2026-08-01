using System.Collections;
using System.Collections.Generic;
using PostMan.Player;
using PostMan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Day6CabinetBagBehaviour : MonoBehaviour
{
    
    public Button extraButton;
    public TextMeshProUGUI content;
    [Header("Subtitle")]
    public string subtitleKey;
    [Header("黑袋背面图片")]
    public Sprite backPhoto;


    void Start()
    {
        extraButton.gameObject.SetActive(true);

        content.text = "查看背面";

        extraButton.onClick.AddListener(CheckBack);
    }

    private void CheckBack()
    {
        ViewImagePanel.Instance.ShowImage(backPhoto , true);

        extraButton.gameObject.SetActive(false);

        extraButton.onClick.RemoveListener(CheckBack);

        SubtitleUI.Instance.TypeSubtitle(subtitleKey);
    }
}
