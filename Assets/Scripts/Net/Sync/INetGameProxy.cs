using System;

namespace Net.Sync
{
    public interface INetGameProxy
    {
        event Action<int> OnGameConnected;
        
        event Action OnGameDisconnected;

        /// <summary>
        /// 服务器下发的当前连接的客户端Token，仅在当前连接有效，不可跨会话
        /// </summary>
        int SessionId { get; }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        void Connect();
        
        /// <summary>
        /// 设置会话ID，外部无需调用
        /// </summary>
        /// <param name="sessionToken"></param>
        void SetSessionToken(int sessionToken);
        
        /// <summary>
        /// 发送消息，传入的消息对象可以不用初始化会话ID，方法内部会自动初始化
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        void Send(Message message, EProtocolChannel channel);
        
        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="netConfig"></param>
        INetGameProxy Init(NetConfig netConfig);
    }
}
