using Core.DI;
using Core.Net.Protocols.Tcp.Messages.Common;
using Core.Net.SyncModule.Interface;
using Net.Protocols;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    /// <summary>
    /// 服务器连接消息处理器
    /// </summary>
    public class S2C_ConnectMessageHandler : MessageHandler<S2C_ConnectMessage>
    {
        [Inject] private IHeartbeatService _heartbeatService;
        
        public override S2C_ConnectMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            // 记录ID
            _netManager.SetSessionToken(Message.SessionID, Message.ClientIds.ToArray());
            // 发送心跳
            _heartbeatService.Start();
        }
    }
}
