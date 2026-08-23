using Core.Net.Protocols.Tcp.Messages.Common;

namespace Net.Protocols.Tcp.Handlers.C2S
{
    public class C2S_LeaveRaceMessageHandler : MessageHandler<LeaveRaceMessage>
    {
        public override LeaveRaceMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            
            
        }
    }
}
