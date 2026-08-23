using Core.Net.Protocols.Tcp.Messages.Common;

namespace Net.Protocols.Tcp.Handlers.C2S
{
    public class C2S_ReconnectRaceMessageHandler : MessageHandler<ReconnectRaceMessage>
    {
        public override ReconnectRaceMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {

        }
    }
}
