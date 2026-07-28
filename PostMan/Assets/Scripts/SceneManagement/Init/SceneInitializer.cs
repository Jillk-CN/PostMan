using PostMan.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PostMan.Scene
{
    public delegate void SceneInitHandler(int sceneOrder, string sceneName);
    /// <summary>
    /// 场景初始化器,场景中的物体将初始化操作提交到这里,等待场景完全加载后调用
    /// </summary>
    public class SceneInitializer : MonoSingleton<SceneInitializer>
    {
        /// <summary>
        /// 场景次序,标识当前是玩家“进入的”第几个场景,不代表项目里的Scene
        /// </summary>
#if DEBUG
        [SerializeField]
#endif
        private int sceneOrder = 0;
        //毕竟在语义上这不能算是个事件,且防止与公开的场景加载到位的事件冲突
        /// <summary>
        /// 获取SceneOrder
        /// </summary>
        public int SceneOrder => sceneOrder;
        private SceneInitHandler initOperation;
        [SerializeField]
        [Tooltip("在切换到这个场景的时候会重置计数")]
        private string resetSceneName;
        private void Awake()
        {
            GameSceneManager.OnSceneSwitchCompleted += OnSceneLoad;
        }

        public void Register(SceneInitHandler operation)
        {
            if (operation == null)
            {
                return;
            }
            if (this.initOperation == null)
            {
                this.initOperation = operation;
            }
            else
            {
                this.initOperation += operation;
            }
        }
        private void OnSceneLoad(IReadOnlyList<string> sceneName)
        {
            /*返回了加载的场景名称,和unity里的一样
            foreach (var item in sceneName)
            {
                Debug.Log(item);    
            }
             */
            this.sceneOrder++;
            if (sceneName[0]==resetSceneName)
            {
                this.sceneOrder = 0;
            }
            this.initOperation?.Invoke(this.sceneOrder, string.Empty);
            Debug.LogWarningFormat("以记载到场景{0}", this.sceneOrder);
            this.initOperation = null;
        }
    }
}
