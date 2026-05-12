using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;

namespace Net.FrameSync.Handler
{
    /// <summary>
    /// ƥ��ɹ���Ϣ������
    /// </summary>
    public class S2C_MatchSuccessMessageHandler : MessageHandler<S2C_MatchSuccessMessage>
    {
        public override S2C_MatchSuccessMessage TcpMessage { get; set; }

        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);
        }
    }
}
