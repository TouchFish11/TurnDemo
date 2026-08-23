using Core.Net.Protocols.Tcp.Messages.Battle.C2S;
using Net.Protocols;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    public class S2C_MatchConfirmMessageHandler : MessageHandler<C2S_MatchConfirmMessage>
    {
        public override C2S_MatchConfirmMessage Message { get; protected set; }
    
        protected override void OnHandle()
        {

        }
    }
}
