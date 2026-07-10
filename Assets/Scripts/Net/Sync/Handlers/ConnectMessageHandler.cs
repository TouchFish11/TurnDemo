using Core.DI;
using Core.Log;
using Net.Sync.Msg.S2C;

namespace Net.Sync.Handlers
{
    /// <summary>
    /// 连接消息处理
    /// </summary>
    public class ConnectMessageHandler : MessageHandler<ConnectMessage>
    {
        public override ConnectMessage Message { get; protected set; }
        
        protected override void OnHandleMessage()
        {
            // 设置当前连接的会话ID
            DIContainer.GetInstance<INetGameProxy>().SetSessionToken(Message.SessionID);
            Logger.LogDebug(TODO, $"[ConnectMessageHandler] 已处理连接消息");
        }
    }
}
