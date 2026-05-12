

using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;

namespace Net.FrameSync.Handler
{
    /// <summary>
    /// ���������Ϳͻ���_������Ϣ������
    /// </summary>
    public class S2C_ConnectMessageHandler : MessageHandler<S2C_ConnectMessage>
    {
        public override S2C_ConnectMessage TcpMessage { get; set; }

        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);
        }
    }
}
