using Core.Net.Protocols.Tcp.Messages.Battle.C2S;

namespace Net.Protocols.Tcp.Handlers.C2S
{
    public class C2S_MatchMessageHandler : MessageHandler<C2S_MatchMessage>
    {
        public override C2S_MatchMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            
            
        }
    }
}
