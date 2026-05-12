using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;

namespace Net.FrameSync.Handler
{
    public class S2C_StartRaceMessageHandler : MessageHandler<S2C_StartRaceMessage>
    {
        public override S2C_StartRaceMessage TcpMessage {  get; set; }

        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);
        }
    }
}
