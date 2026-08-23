using Net.Protocols;
using Net.Protocols.Tcp.Messages.Chat;

namespace Core.Net.Protocols.FSync.Handlers
{
    /// <summary>
    /// 聊天消息处理器
    /// </summary>
    public class ChatMessageHandler : MessageHandler<ChatMessage>
    {
        public override ChatMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            //var controller = DIContainer.GetInstance<IUIManager>().GetController<MainController>();
            //controller.AddChat(Message.SessionID, Message.ChatMsg);
        }
    }
}
