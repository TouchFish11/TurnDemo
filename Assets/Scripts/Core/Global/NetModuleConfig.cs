using System;
using UnityEngine;

namespace Core.Global
{
    /// <summary>
    /// 网络模块配置
    /// </summary>
    [Serializable]
    public class NetModuleConfig
    {
        /// <summary>
        /// UDP接收缓冲区大小
        /// </summary>
        [Header("UDP接收缓冲区大小")]
        [Tooltip("缓存帧同步数据包")]
        public short udpReceiveBufferSize = 8192;

        /// <summary>
        /// TCP接收临时缓冲区大小
        /// </summary>
        [Header("TCP发送缓冲区大小")]
        [Tooltip("临时缓存接收的TCP消息")]
        public short tcpReceiveTempBufferSize = 1024;

        /// <summary>
        /// TCP接收缓冲区大小
        /// </summary>
        [Header("UDP接收缓冲区大小")]
        [Tooltip("缓存接收的TCP待处理消息")]
        public short tcpReceiveBufferSize = 1024;

        /// <summary>
        /// 心跳消息发送间隔时间（ms）
        /// </summary>
        [Header("心跳消息发送间隔时间（ms）")]
        [Tooltip("心跳消息发送间隔（ms）")]
        public short heartMsgSendIntervalTime = 3000;
    }
}
