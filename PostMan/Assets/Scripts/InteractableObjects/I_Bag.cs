using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using UnityEngine.UIElements;
using PostMan.AudioSystem;
using UnityEngine.SceneManagement;

public class I_Bag : MonoBehaviour, IInteractable
{
    [Header("背包手持位置设置（调节偏移）")]
    public float X = 0f;
    public float Y = 0f;
    public float Z = 0f;
    public float rotateX = 0f;
    public float rotateY = 0f;
    public float rotateZ = 0f;
    [Header("放下按键")]
    public KeyCode QuitKey;
    private bool inHand = false;
    private Transform Player;
    private Quaternion defaultRota;

    [Header("交互设置")]
    public bool canInteract;
    public int priority;
    public bool CanInteract { get => canInteract; set => canInteract=value; }
    public int Priority { get => priority; set => priority=value; }
    public AudioClip takeSound;
    public AudioClip putSound;

    [Header("晃动设置")]
    public AudioClip shakeSound;
    [Tooltip("晃动音效循环播放间隔（秒）")]
    public float shakeSoundLoopInterval = 0.5f;
    [Tooltip("晃动音效音量（0~1），外部可实时修改")]
    public float shakeSoundVolume = 1f;
    [Tooltip("晃动剧烈程度（位置偏移幅度，旋转偏移为 intensity * 10 度）")]
    public float shakeIntensity = 0.1f;
    private bool isShaking = false;
    private Coroutine shakeCoroutine;
    private Coroutine shakeSoundCoroutine;
    private Vector3 groundBasePos;
    private Quaternion groundBaseRot;

    void Start()
    {
        defaultRota = gameObject.transform.rotation;
    }

    void Update()
    {   
        if(Input.GetKeyDown(QuitKey) && inHand)
        {
            QuitBag();
        }
        
        if(inHand && !isShaking)
        {
            gameObject.transform.localPosition = new Vector3(X , Y ,Z);
            gameObject.transform.rotation = Quaternion.Euler(rotateX ,Player.transform.rotation.eulerAngles.y + rotateY , rotateZ);
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        if(CanInteract == false) return;

        if(takeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , takeSound , false , false , 0f , 1f);
        }

        Player = player.gameObject.transform;

        gameObject.transform.parent = player.gameObject.transform;

        gameObject.transform.rotation = Quaternion.Euler(-90 ,-180 , 90);

        inHand = true;
    }

    public GameObject QuitBag()
    {
        if(putSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , putSound , false , false , 0f , 1f);
        }

        inHand = false;
    
        gameObject.transform.parent = null;

        gameObject.transform.position = new Vector3(gameObject.transform.position.x , 0.617f , gameObject.transform.position.z);

        gameObject.transform.rotation = defaultRota;

        return gameObject;
    }

    /// <summary>
    /// 开启晃动效果（持续晃动，直到调用 StopShake 停止）
    /// </summary>
    public void StartShake()
    {
        if (isShaking) return;

        isShaking = true;

        // 记录地上状态的初始世界坐标与旋转
        groundBasePos = transform.position;
        groundBaseRot = transform.rotation;

        // 启动晃动音效循环播放
        if (shakeSound != null)
        {
            if (shakeSoundCoroutine != null)
            {
                StopCoroutine(shakeSoundCoroutine);
            }
            shakeSoundCoroutine = StartCoroutine(PlayShakeSoundLoop());
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    /// <summary>
    /// 关闭晃动效果，恢复初始位置与旋转
    /// </summary>
    public void StopShake()
    {
        if (!isShaking) return;

        // 停止晃动音效循环
        if (shakeSoundCoroutine != null)
        {
            StopCoroutine(shakeSoundCoroutine);
            shakeSoundCoroutine = null;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        // 恢复初始状态
        if (inHand)
        {
            transform.localPosition = new Vector3(X, Y, Z);
            transform.rotation = Quaternion.Euler(rotateX, Player.transform.rotation.eulerAngles.y + rotateY, rotateZ);
        }
        else
        {
            transform.position = groundBasePos;
            transform.rotation = groundBaseRot;
        }

        isShaking = false;
    }

    private IEnumerator ShakeCoroutine()
    {
        while (true)
        {
            // 晃动剧烈程度固定使用 shakeIntensity
            float intensity = shakeIntensity;

            if (inHand)
            {
                // 手上状态：基于挂载点（玩家）的局部坐标晃动
                Vector3 basePos = new Vector3(X, Y, Z);
                Quaternion baseRot = Quaternion.Euler(rotateX, Player.transform.rotation.eulerAngles.y + rotateY, rotateZ);

                transform.localPosition = basePos + Random.insideUnitSphere * intensity;
                transform.rotation = baseRot * Quaternion.Euler(
                    Random.Range(-intensity * 10f, intensity * 10f),
                    Random.Range(-intensity * 10f, intensity * 10f),
                    Random.Range(-intensity * 10f, intensity * 10f)
                );
            }
            else
            {
                // 地上状态：基于世界坐标晃动
                transform.position = groundBasePos + Random.insideUnitSphere * intensity;
                transform.rotation = groundBaseRot * Quaternion.Euler(
                    Random.Range(-intensity * 10f, intensity * 10f),
                    Random.Range(-intensity * 10f, intensity * 10f),
                    Random.Range(-intensity * 10f, intensity * 10f)
                );
            }

            yield return null;
        }
    }

    /// <summary>
    /// 晃动音效循环播放协程（循环间隔可控）
    /// </summary>
    private IEnumerator PlayShakeSoundLoop()
    {
        while (true)
        {
            // 每次播放时读取 shakeSoundVolume，外部修改后实时生效
            AudioManager.Instance.Play(AudioTrackId.FX, shakeSound, false, false, 0f, shakeSoundVolume);
            yield return new WaitForSeconds(shakeSoundLoopInterval);
        }
    }
}