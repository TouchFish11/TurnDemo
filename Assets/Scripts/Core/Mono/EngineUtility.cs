using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Mono
{
    /// <summary>
    /// 引擎工具类
    /// 对引擎方法进行封装
    /// </summary>
    public static class EngineUtility
    {
        /// <summary>
        /// 创建Unity游戏对象
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="types">创建时附加的组件类型</param>
        /// <returns></returns>
        public static GameObject Create(string name, params Type[] types)
        {
            return new GameObject(name, types);
        }
        
        /// <summary>、
        /// 移除一个游戏对象、组件或资源
        /// </summary>
        /// <param name="obj">要销毁的对象在销毁对象之前可选择延迟的时间。</param>
        /// <param name="time">在销毁对象之前可选择延迟的时间，-1为不指定延迟时间</param>
        public static void Destroy(Object obj, float time = -1)
        {
            if (Mathf.Approximately(time, -1))
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.Destroy(obj, time);
            }
        }

        /// <summary>
        /// 实例化游戏对象
        /// </summary>
        /// <param name="original"></param>
        /// <param name="parent"></param>
        /// <param name="instantiateInWorldSpace"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instantiate<T>(T original, Transform parent = null, bool instantiateInWorldSpace = false) where T : Object
        {
            return Object.Instantiate(original, parent, instantiateInWorldSpace);
        }
    }
}
