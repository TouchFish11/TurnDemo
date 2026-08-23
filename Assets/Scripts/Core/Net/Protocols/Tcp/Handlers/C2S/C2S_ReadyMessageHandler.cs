using Core.Net.Protocols.Tcp.Messages.Battle.C2S;

namespace Net.Protocols.Tcp.Handlers.C2S
{
    public class C2S_ReadyMessageHandler : MessageHandler<C2S_ReadyMessage>
    {
        public override C2S_ReadyMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            
            
        }
    }
}
