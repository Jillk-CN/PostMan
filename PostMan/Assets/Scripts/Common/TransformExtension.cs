using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostMan.Common
{
    /// <summary>
    /// Transform扩展类,提供一些有用的方法
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// 未知层级查找后代物体的Transform组件,不要频繁调用
        /// </summary>
        /// <param name="fatherTF"></param>
        /// <param name="childName">子物体的名称,必须完全一致</param>
        /// <returns></returns>
        public static Transform FindChildByName(this Transform fatherTF,  string childName)
        {
            //递归查找
            Transform childTF=fatherTF.Find(childName);
            if(childTF ==null)
            {
                for (int i = 0; i < fatherTF.childCount; i++)
                {
                   childTF= FindChildByName(fatherTF.GetChild(i), childName);
                    if (childTF != null)
                    {
                        break;
                    }
                }
            }
            return childTF;
        }
    }
}