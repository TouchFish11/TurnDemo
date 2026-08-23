using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events.Net;
using Core.Net.Protocols.Tcp.Messages.Common;
using Net.Protocols;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    public class S2C_DisConnectMessageHandler : MessageHandler<S2C_DisConnectMessage>
    {
        [Inject] private IEventCenter _eventCenter;
        
        public override S2C_DisConnectMessage Message { get; protected set; }
    
        protected override void OnHandle()
        {
            var playerDisconnectedEvent = EventSource.Get<PlayerDisconnectedEvent>();
            playerDisconnectedEvent.DisconnectionId = Message.SessionID;
            _eventCenter.TriggerEvent(playerDisconnectedEvent);
        }
    }
}
