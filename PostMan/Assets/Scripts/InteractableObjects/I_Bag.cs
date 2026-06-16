using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Player;
using PostMan.Common;
using UnityEngine.UIElements;
using PostMan.AudioSystem;

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
        
        if(inHand)
        {
            gameObject.transform.localPosition = new Vector3(X , Y ,Z);
            gameObject.transform.rotation = Quaternion.Euler(rotateX ,Player.transform.rotation.eulerAngles.y + rotateY , rotateZ);
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        Debug.LogFormat("[I_Bag.cs] 与黑袋交互");

        if(takeSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , takeSound , false , false , 0f , 1f);
        }

        Player = player.gameObject.transform;

        gameObject.transform.parent = player.gameObject.transform;

        gameObject.transform.rotation = Quaternion.Euler(-90 ,-180 , 90);

        inHand = true;
    }

    private void QuitBag()
    {
        if(putSound != null)
        {
            AudioManager.Instance.Play(AudioTrackId.FX , putSound , false , false , 0f , 1f);
        }

        inHand = false;
    
        gameObject.transform.parent = null;

        gameObject.transform.position = new Vector3(gameObject.transform.position.x , 0.617f , gameObject.transform.position.z);

        gameObject.transform.rotation = defaultRota;
    }
}
