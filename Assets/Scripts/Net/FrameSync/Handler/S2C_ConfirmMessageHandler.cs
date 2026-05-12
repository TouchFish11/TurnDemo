using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;

namespace Net.FrameSync.Handler
{
    public class S2C_ConfirmMessageHandler : MessageHandler<S2C_ConfirmMessage>
    {
        public override S2C_ConfirmMessage TcpMessage { get; set; }

        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);
        }
    }
}
