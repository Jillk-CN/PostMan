using PostMan.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetSceneOrder : MonoBehaviour
{
    private void Awake()
    {
    }
    private void Update()
    {
        SceneInitializer.Instance.SetSceneOrder(0);
    }
}
