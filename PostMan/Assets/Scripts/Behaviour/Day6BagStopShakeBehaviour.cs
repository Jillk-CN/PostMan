using System.Collections;
using System.Collections.Generic;
using PostMan.Scene;
using UnityEngine.SceneManagement;
using UnityEngine;
using PostMan.Player;

public class Day6BagStopShakeBehaviour : MonoBehaviour
{
    [Header("SceneOrder")]
    public int sceneOrder;
    private I_Bag i_Bag;
    private Transform player;
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
                player = obj.transform;

                i_Bag = obj.GetComponentInChildren<I_Bag>();

                break;
            }
        }

        // 空值检查
        if (player == null)
        {
            Debug.LogWarning("[Day6BagStopShakeBehaviour] 未找到 Player 对象，功能已禁用");
            enabled = false;
            return;
        }
        if (i_Bag == null)
        {
            Debug.LogWarning("[Day6BagStopShakeBehaviour] 未找到 I_Bag 组件，功能已禁用");
            enabled = false;
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        i_Bag.StopShake();
        
        PlayerMotion playerMotion = player.GetComponent<PlayerMotion>();

        if (playerMotion != null)
        {
            playerMotion.walkSpeed = 3f;
        }
    }
}
