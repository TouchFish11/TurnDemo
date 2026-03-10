using UnityEngine;

namespace Core.Mono
{
    /// <summary>
    /// 引擎工具类
    /// 对引擎方法进行封装
    /// </summary>
    public static class EngineUtility
    {
        /// <summary>、
        /// 销毁
        /// 移除一个游戏对象、组件或资源。
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
    }
}
