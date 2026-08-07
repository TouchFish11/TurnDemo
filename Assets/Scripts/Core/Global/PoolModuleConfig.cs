using System;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 对象池模块配置
    /// </summary>
    [Serializable]
    public class PoolModuleConfig
    {
        /// <summary>
        /// 是否启用缓存池布局――开发阶段使用
        /// </summary>
        [Header("启用对象池层级结构")]
        [Tooltip("对象池对象按层级结构布局")]
        public bool isOpenLayout = true;

        /// <summary>
        /// 活跃时间阈值
        /// </summary>
        [Header("活跃时间阈值")] 
        [Tooltip("大于该数值为惰性，小于为活跃")]
        public float activeTimeThreshold = 60;

        /// <summary>
        /// 池子统一最小阈值
        /// </summary>
        [Header("池子统一最小阈值")] 
        [Tooltip("对于修改释放策略，保留的缓存对象数")]
        public int poolMinSize = 10;

        /// <summary>
        /// 池子统一最大阈值
        /// </summary>
        [Header("池子统一最大阈值")] 
        [Tooltip("池子最大对象缓存数，大于则扩容")]
        public int poolMaxSize = 50;
    }
}
