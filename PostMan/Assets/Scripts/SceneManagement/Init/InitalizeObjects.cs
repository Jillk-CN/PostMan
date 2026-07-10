using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Scene
{
    public class InitalizeObjects : MonoBehaviour
    {
        [Header("初始化参数,编辑器内手动配置")]
        [SerializeField]
        [Tooltip("这些物体刚进入场景时显示吗")]
        private bool activeOnInit;
        [SerializeField]
        [Tooltip("玩家在哪个场景时,应用这个初始化")]
        private int targetSceneOrder;
        [SerializeField]
        private GameObject[] objects;
        private void OnEnable()
        {
            SceneInitializer.Instance.Register(Init); 
        }
        private void Init(int sceneOrder,string sceneName)
        {
            if (targetSceneOrder!=sceneOrder)
            {
                return;
            }
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(activeOnInit);
            }
        }

    }
}
