using Core.Net.FrameSync.Manager;
using Core.Net.Tcp.Message;
using Core.Net.Tcp.Message.S2C;

namespace Core.Net.Tcp.Handler
{
    /// <summary>
    /// ������Ϣ������
    /// </summary>
    public class S2C_HeartMessageHandler : MessageHandler<S2C_HeartMessage>
    {
        public override S2C_HeartMessage TcpMessage { get; set; }


        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);

            NetManager.Instance.GetTcpClient().CalcTcpRTT();
        }
    }
}
