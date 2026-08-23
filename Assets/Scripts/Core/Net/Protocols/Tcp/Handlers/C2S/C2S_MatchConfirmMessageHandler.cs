using Core.Net.Protocols.Tcp.Messages.Battle.C2S;

namespace Net.Protocols.Tcp.Handlers.C2S
{
    public class C2S_MatchConfirmMessageHandler : MessageHandler<C2S_MatchConfirmMessage>
    {
        public override C2S_MatchConfirmMessage Message { get; protected set; }
    
        protected override void OnHandle()
        {
            
        }
    }
}
