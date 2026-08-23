using System;

namespace Net.Protocols
{
    /// <summary>
    /// 协议通道
    /// </summary>
    public enum EProtocolChannel : byte
    {
        /// <summary>
        /// 处理有自定义消息头的消息类型
        /// </summary>
        Resolve = 1,
        
        /// <summary>
        /// 处理没有自定义消息头的消息类型
        /// </summary>
        [Obsolete("Raw channel is deprecated, please use Resolve channel instead")]
        Raw = 2
    }
}
