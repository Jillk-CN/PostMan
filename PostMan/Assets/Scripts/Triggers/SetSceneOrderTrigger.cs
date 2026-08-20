using PostMan.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Trigger
{
    //事实证明靠场景计数做场景设置是不能做到易于修改,
    //如果可以,应围绕任务来做场景设置,任务系统建议有一套可行的通知功能,
    //其它部分根据任务内容做好自身设置
    public class SetSceneOrderTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool triggerOnce=true;
        [SerializeField]
        private bool canTrigger=true;

        public int sceneOrderToSet;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")&&canTrigger)
            {
                SceneInitializer.Instance.SetSceneOrder(sceneOrderToSet);
                if (triggerOnce)
                {
                    this.canTrigger = false;
                }
            }
        }
        
    }
}
