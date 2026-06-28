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
        private SceneInitHandler initOperation;//换种方式写,防止与公开的场景加载到位的事件冲突
        private void Awake()
        {

        }
        //测试用
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard.iKey.wasPressedThisFrame)
            {
                OnSceneLoad();
            }
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
        private void OnSceneLoad()
        {
            this.sceneOrder++;//TODO : 如果是主菜单,需要设置为0
            this.initOperation?.Invoke(this.sceneOrder, string.Empty);
            this.initOperation = null;
        }
    }
}
