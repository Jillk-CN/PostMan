using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PostMan.Scene;

public class ColliderDelayMovingAfterTask : MonoBehaviour
{
    public MovePlayerAfterTask movePlayerAfterTask;

    public int sceneOrder;
    private Coroutine currentCoroutine;

    void OnTriggerEnter(Collider other)
    {
        if(sceneOrder != SceneInitializer.Instance.SceneOrder)return;

        movePlayerAfterTask.DelayMovingPlayerAfterTaskCompletion();
    }
}
