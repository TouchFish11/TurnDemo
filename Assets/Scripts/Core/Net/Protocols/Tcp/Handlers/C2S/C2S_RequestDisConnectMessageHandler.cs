using Core.Net.Protocols.Tcp.Messages.Common;

namespace Net.Protocols.Tcp.Handlers.C2S
{
    public class C2S_RequestDisConnectMessageHandler : MessageHandler<C2S_RequestDisConnectMessage>
    {
        public override C2S_RequestDisConnectMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {

            
        }
    }
}
