using System;
using Core.Log;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 日志模块配置
    /// </summary>
    [Serializable]
    public class LogModuleConfig
    {
        /// <summary>
        /// 日志标签与日志过滤级别共同作用
        /// </summary>
        [Header("日志标签")]
        [Tooltip("没有选中的日志标签将不会被记录")]
        public ELogTags tag = ~ELogTags.None;

        /// <summary>
        /// 日志写入最大间隔时间（s）
        /// </summary>
        [Header("日志写入最大间隔时间")]
        [Tooltip("到达时间将写入一次日志到本地")]
        public ushort writeLogMaxIntervalTime = 30;

        [Header("日志过滤级别")] 
        [Tooltip("没有选中的日志类型将不会被记录")]
        public ELogLevel filterLevel = ~ELogLevel.None;
    }
}
