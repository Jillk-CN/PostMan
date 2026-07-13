using PostMan.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Scene
{
    //由于信件是与床交互后显示的,所以需要设置显示什么信件,当然,这个类写的很差劲
    public class InitializeLetter : MonoBehaviour
    {
        [Header("初始化参数,编辑器内手动配置")]
        [SerializeField]
        [Tooltip("玩家在哪个场景时,应用这个初始化")]
        private int targetSceneOrder;
        [Header("要显示的信件")]
        [SerializeField]
        private InteractablesProcessListSO processListSO;
        [SerializeField]
        private SetOtherInteractables showLetterInteractable;
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
            showLetterInteractable.processListSO = this.processListSO;
        }

    }
}
