using System;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 更新模块配置
    /// </summary>
    [Serializable]
    public class UpdateModuleConfig
    {
        /// <summary>
        /// 资源服务器地址
        /// </summary>
        [Header("资源服务器地址")]
        [Tooltip("服务器资源下载路径")]
        public string resServerIp = "http://ip:port/...";

        /// <summary>
        /// 单次更新中对比文件重新下载最大次数
        /// </summary>
        [Header("对比文件重新下载最大次数")]
        [Tooltip("对比文件最大重试次数（0为无限制）")]
        public int reDownloadCompareFileMaxNum = 5;

        /// <summary>
        /// 单次更新中AB包重新下载最大次数
        /// </summary>
        [Header("AB包重新下载最大次数")]
        [Tooltip("AB包重试下载最大次数（0为无限制）")]
        public int reDownloadAbMaxNum = 5;

        /// <summary>
        /// 最大并发数
        /// </summary>
        [Header("最大下载并发数")]
        [Tooltip("最大下载并发数")]
        public int maxConcurrencyNum = 8;

        /// <summary>
        /// 连接超时（s）
        /// </summary>
        [Header("连接超时")]
        [Tooltip("建立服务器连接的最大等待时间（s），0为无限制")]
        public int connectTimeout = 60;

        /// <summary>
        /// 单文件最大重试次数
        /// </summary>
        [Header("单文件最大重试次数")]
        [Tooltip("单文件最大重试次数（连接失败+下载失败）")]
        public int maxRetryCount = 5;

        /// <summary>
        /// 最大重试等待时间
        /// </summary>
        [Header("最大重试等待时间")]
        [Tooltip("重试前等待一段时间，避免频繁请求")]
        public float maxRetryWaitSeconds = 5f;

        /// <summary>
        /// 速度更新间隔
        /// </summary>
        [Header("速度更新间隔")]
        [Tooltip("单位时间内的下载量")]
        public float speedUpdateInterval = 1f;
    }
}
