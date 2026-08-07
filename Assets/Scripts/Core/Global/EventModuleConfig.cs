using System;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EventModuleConfig
    {
        /// <summary>
        /// 每帧允许触发的最大延迟事件数量
        /// </summary>
        [Header("每帧允许触发的最大延迟事件数量")]
        [Tooltip("限制阈值，防止单帧处理过多事件导致帧率下降")]
        [Range(1, 500)]
        public int eventTriggerMaxNumPerFrame = 10;
    }
}
