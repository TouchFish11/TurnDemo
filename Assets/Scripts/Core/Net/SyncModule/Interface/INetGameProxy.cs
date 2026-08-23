using System;
using Core.Net.SyncModule.Manager;
using Net.Protocols;

namespace Core.Net.SyncModule.Interface
{
    public interface INetGameProxy
    {
        /// <summary>
        /// 服务器下发的客户端ID
        /// </summary>
        int ClientId { get; }
        
        /// <summary>
        /// 业务层使用的连接完成事件
        /// </summary>
        event Action<int, int[]> OnConnected;
        
        /// <summary>
        /// 业务层使用的连接断开事件
        /// </summary>
        event Action OnDisconnected;

        /// <summary>
        /// 连接到服务器
        /// </summary>
        void Connect();
        
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

        /// <summary>
        /// TCP的延迟时间回调（ms）
        /// </summary>
        event Action<long> TcpRtt;
    }
}
