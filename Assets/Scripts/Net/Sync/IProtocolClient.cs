using System;

namespace Net.Sync
{
    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IProtocolClient
    {
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
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        void Send(byte[] data, EProtocolChannel channel);

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
