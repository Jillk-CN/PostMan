using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostMan.AudioSystem;
using PostMan.Common;
using PostMan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Box410Fallen : MonoBehaviour
{
    public GameObject imagePanel;
    public Button button;
    public string subtitleKey1;
    public string subtitleKey2;
    public AudioClip knockSFX;
    [Header("Day2 410掉落箱子侧面凹陷处图片")]
    public Sprite sprite;
    private ImageViewer imageViewer;
    private Transform checkButton;

    void OnEnable()
    {
        imageViewer = imagePanel.transform.GetComponentInChildren<ImageViewer>();

        checkButton = imagePanel.transform.FindChildByName("extraButton (1)");

        checkButton.FindChildByName("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "查看侧面";

        checkButton.gameObject.SetActive(true);

        button.onClick.AddListener(TransSprite);
    }

    private void TransSprite()
    {
        imageViewer.SetImage(sprite);

        SubtitleUI.Instance.TypeSubtitle(subtitleKey1);

        StartCoroutine(Behavior());

        button.onClick.RemoveListener(TransSprite);

        checkButton.gameObject.SetActive(false);
    }

    private IEnumerator Behavior()
    {
        yield return new WaitForSeconds(2f);

        if(knockSFX != null)AudioManager.Instance.Play(AudioTrackId.FX , knockSFX);
        
        if(subtitleKey2 !="")SubtitleUI.Instance.TypeSubtitle(subtitleKey2 , 0.7f);

        yield return new WaitForSeconds(6f);

        TaskManager.Instance.AdvanceTask(7 , 1);
    }

}
