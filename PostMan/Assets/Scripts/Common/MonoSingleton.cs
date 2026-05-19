using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Common 
{
    /// <summary>
    /// 脚本单例类
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if(instance ==null )
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        return null;
                       //instance = new GameObject("MonoSingleton of " + typeof(T).Name).AddComponent<T>();
                       //instance.Init();
                    }
                    else
                    {
                        instance.Init();
                    }
                }
                return instance;
            }
        }
        protected virtual void Init(){}
    }
}