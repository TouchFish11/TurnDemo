using System;
using Net.Protocols;

namespace Core.Net.SyncModule.Clients
{
    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IProtocolClient
    {
        bool IsConnected { get; }
        
        /// <summary>
        /// 接收消息数据回调，获取的是完整的一条消息
        /// </summary>
        event Action<byte[], EProtocolChannel> OnDataReceived;
        
        event Action OnConnected;
        
        event Action OnDisconnected;
        
        event Action<string> OnError;

        /// <summary>
        /// 连接到指定IP和端口的服务器
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        void Connect(string serverIp, ushort serverPort);
        
        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        void SendAsync(byte[] data, EProtocolChannel channel);

        /// <summary>
        /// 周期性触发，驱动协议工作
        /// </summary>
        void Tick();
        
        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();
    }
}
