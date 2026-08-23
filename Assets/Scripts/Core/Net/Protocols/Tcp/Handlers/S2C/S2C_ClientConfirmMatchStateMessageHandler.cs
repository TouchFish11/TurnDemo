using Net.Protocols;
using Net.Protocols.Tcp.Messages.Battle.S2C;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    public class S2C_ClientConfirmMatchStateMessageHandler : MessageHandler<S2C_ClientConfirmMatchStateMessage>
    {
        public override S2C_ClientConfirmMatchStateMessage Message { get; protected set; }
    
        protected override void OnHandle()
        {
            
        }
    }
}
