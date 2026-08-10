using System.Collections;
using System.Collections.Generic;
using PostMan.Scene;
using PostMan.Player;
using UnityEngine.SceneManagement;
using UnityEngine;
using PostMan.Common;
using Cinemachine;
using PostMan.InputManagement;
using PostMan.UI;

public class Day6MailBox410Behaviour : MonoBehaviour , IInteractable
{
    public MovePlayerAfterTask movePlayerAfterTask31;
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
        // 查找背包（SceneOrder 可能尚未就绪，这里只做查找，不强制禁用）
        FindBag();
    }

    private void FindBag()
    {
        persistent = SceneManager.GetSceneByName("Persistent");

        if (!persistent.IsValid()) return;

        GameObject[] rootObjects = persistent.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "Player")
            {
                i_Bag = obj.GetComponentInChildren<I_Bag>();

                break;
            }
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        // 确保 i_Bag 非空，为空时重新查找
        if (i_Bag == null)
        {
            FindBag();
        }

        if (i_Bag == null)
        {
            Debug.LogWarning("[Day6MailBox410Behaviour] 未找到 I_Bag 组件，无法执行交互");
            return;
        }

        Bag = i_Bag.QuitBag();

        i_Bag = Bag.GetComponent<I_Bag>();

        Bag.SetActive(false);

        Bag.transform.position = new Vector3(Bag.transform.position.x ,Bag.transform.position.y + 0.818f ,Bag.transform.position.z);

        StartCoroutine(Behaviour(player));
    }

    private IEnumerator Behaviour(PlayerInteractor player)
    {
        yield return new WaitForSeconds(3f);

        gameObject.GetComponent<I_MailBox>().OpenDoorPublic();

        Bag.SetActive(true);

        GameObject blackHand = Bag.transform.FindChildByName("Black_Hand_m").gameObject;

        Animator blackHandAnimator = blackHand.GetComponent<Animator>();

        CinemachineVirtualCamera bagVirtualCamera = Bag.transform.FindChildByName("BagVirtualCamera").GetComponent<CinemachineVirtualCamera>();

        CinemachineVirtualCamera playerVirtualCamera = player.gameObject.transform.FindChildByName("FPVcam").GetComponent<CinemachineVirtualCamera>();

        GameInputManager.Instance.SetPlayerAllInput(false);

        bagVirtualCamera.enabled = true;

        playerVirtualCamera.enabled = false;

        yield return new WaitForSeconds(2f);

        blackHand.SetActive(true);

        blackHandAnimator.SetTrigger("FaceJump");

        yield return new WaitForSeconds(1.12f);
        
        BlackScreen.Instance.BlackIn("" , 0.1f);

        yield return new WaitForSeconds(0.2f);

        movePlayerAfterTask31.DelayMovingPlayerAfterTaskCompletion();

        yield return new WaitForSeconds(0.5f);

        blackHandAnimator.ResetTrigger("FaceJump");
    }
}
