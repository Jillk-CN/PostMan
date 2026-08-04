using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine;
using PostMan.Scene;
using Unity.VisualScripting;
using PostMan.Player;

public class Day6BagBehaviour : MonoBehaviour
{
    [Header("SceneOrder")]
    public int sceneOrder;
    [Header("410Mail")]
    public GameObject mailBox410;
    [Header("距离映射设置")]
    [Tooltip("最大距离，用于将 distance 映射到音量/速度变化范围")]
    public float maxDistance = 20f;
    [Tooltip("walkSpeed 最小值（必须大于0）")]
    public float minWalkSpeed = 0.5f;
    [Header("初始值（复位用）")]
    [Tooltip("起始晃动音效音量（对应 minDistance）")]
    public float baseShakeSoundVolume = 0.5f;
    [Tooltip("walkSpeed 最大值（对应 minDistance）")]
    public float maxWalkSpeed = 3f;
    private bool hasTriggered = false;
    private I_Bag i_Bag;
    private PlayerMotion playerMotion;
    private Transform player;
    private bool isTrigger = false;
    private float distance;
    private float minDistance;
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

                playerMotion = obj.GetComponent<PlayerMotion>();

                break;
            }
        }

        // 空值检查
        if (player == null)
        {
            Debug.LogWarning("[Day6BagBehaviour] 未找到 Player 对象，功能已禁用");
            enabled = false;
            return;
        }
        if (i_Bag == null)
        {
            Debug.LogWarning("[Day6BagBehaviour] 未找到 I_Bag 组件，功能已禁用");
            enabled = false;
            return;
        }
        if (playerMotion == null)
        {
            Debug.LogWarning("[Day6BagBehaviour] 未找到 PlayerMotion 组件，功能已禁用");
            enabled = false;
            return;
        }
        if (mailBox410 == null)
        {
            Debug.LogWarning("[Day6BagBehaviour] mailBox410 未赋值，功能已禁用");
            enabled = false;
            return;
        }

        minDistance = Vector3.Distance(gameObject.transform.position , mailBox410.transform.position);
    }

    void Update()
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        if (player == null || mailBox410 == null || isTrigger == false) return;

        distance = Vector3.Distance(player.position , mailBox410.transform.position);

        if(distance < minDistance)distance = minDistance;

        // 归一化距离比例（minDistance -> 0，maxDistance -> 1）
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // 音量：distance = minDistance 时为 baseShakeSoundVolume(0.5)，随 distance 增大而增大，最大 1
        i_Bag.shakeSoundVolume = Mathf.Lerp(baseShakeSoundVolume, 1f, t);

        // walkSpeed：distance = minDistance 时为 maxWalkSpeed(3)，随 distance 增大而减小，最小 minWalkSpeed(>0)
        playerMotion.walkSpeed = Mathf.Lerp(maxWalkSpeed, minWalkSpeed, t);
    }

    void OnTriggerEnter(Collider other)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        if (hasTriggered) return;

        if (i_Bag == null) return;

        isTrigger = true;

        hasTriggered = true;

        i_Bag.StartShake();
    }

    /// <summary>
    /// 强制复位因 distance 改变而改变的音量与速度，恢复为初始值
    /// </summary>
    public void ResetDistanceValues()
    {
        if (i_Bag != null)
        {
            i_Bag.shakeSoundVolume = baseShakeSoundVolume;
        }
        if (playerMotion != null)
        {
            playerMotion.walkSpeed = maxWalkSpeed;
        }
    }
}