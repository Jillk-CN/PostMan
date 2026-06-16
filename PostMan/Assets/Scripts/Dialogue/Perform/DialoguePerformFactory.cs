using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Dialogue
{
    /// <summary>
    /// 演出效果实现类工厂,从这里获取实现类实例
    /// </summary>
    public class DialoguePerformFactory
    {
        //反射消耗性能,简单地缓存一下,假设不会同时使用同一个对象(暂时先这样定)
        private Dictionary<string, DialoguePerform> cache;

        public DialoguePerformFactory()
        {
            cache = new Dictionary<string, DialoguePerform>();
        }

        /// <summary>
        /// 获取演出实现类的实例
        /// </summary>
        /// <param name="name">实现类名称</param>
        /// <returns>实现类实例</returns>
        public DialoguePerform GetPerform(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (cache.ContainsKey(name))
            {
                return cache[name];
            }
            //这个字符串与规定的命名规范一致
            object instance = Activator.CreateInstance(Type.GetType
                ("PostMan.Dialogue." + name));
            if (instance == null)
            {
                return null;
            }
            DialoguePerform perform = instance as DialoguePerform;
            cache.Add(name, perform);
            return perform;
        }
    }
}
