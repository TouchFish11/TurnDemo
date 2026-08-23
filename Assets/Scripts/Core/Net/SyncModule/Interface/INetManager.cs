using System;
using Core.Net.SyncModule.Manager;
using Net.Protocols;

namespace Core.Net.SyncModule.Interface
{
    /// <summary>
    /// 网络管理器接口
    /// </summary>
    public interface INetManager
    {
        event Action<Message, EProtocolChannel> OnMessageReceived;
        
        event Action<int, int[]> OnConnected;
        
        event Action OnDisconnected;
        
        event Action<string> OnError;

        /// <summary>
        /// 服务器下发的当前连接的客户端ID，仅在当前连接有效，不可跨会话
        /// </summary>
        int SessionId { get; }

        void Init(NetConfig config);

        /// <summary>
        /// 设置会话ID，外部无需调用
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <param name="clientIds"></param>
        void SetSessionToken(int sessionToken, int[] clientIds);
        
        void Connect();
        
        void Send(Message message, EProtocolChannel channel);

        void Disconnect();
    }
}
