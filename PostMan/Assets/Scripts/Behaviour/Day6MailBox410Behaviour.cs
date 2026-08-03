using System.Collections;
using System.Collections.Generic;
using PostMan.Scene;
using PostMan.Player;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Day6MailBox410Behaviour : MonoBehaviour , IInteractable
{
    [Header("SceneOrder")]
    public int sceneOrder;
    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    private I_Bag i_Bag;
    private GameObject Bag;
    private UnityEngine.SceneManagement.Scene persistent;

    
    void Start()
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;
        
        persistent = SceneManager.GetSceneByName("Persistent");

        GameObject[] rootObjects = persistent.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "Player")
            {
                i_Bag = obj.GetComponentInChildren<I_Bag>();

                break;
            }
        }

        // 空值检查
        if (i_Bag == null)
        {
            Debug.LogWarning("[Day6MailBox410Behaviour] 未找到 I_Bag 组件，功能已禁用");
            enabled = false;
            return;
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        Bag = i_Bag.QuitBag();

        i_Bag = Bag.GetComponent<I_Bag>();

        Bag.SetActive(false);

        StartCoroutine(Behaviour());
    }

    private IEnumerator Behaviour()
    {
        yield return new WaitForSeconds(2f);

        gameObject.GetComponent<I_MailBox>().OpenDoorPublic();

        Bag.SetActive(true);

    }
}
