using Core.DI;
using Core.Net.Protocols.Tcp.Messages.Common;
using Core.Net.SyncModule.Interface;
using Net.Protocols;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    public class S2C_HeartMessageHandler : MessageHandler<HeartMessage>
    {
        [Inject] private IHeartbeatService _heartbeatService;
        
        public override HeartMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            // 计算rtt
            _heartbeatService.CalcRtt(Message.ClientTimeStamp);
        }
    }
}
