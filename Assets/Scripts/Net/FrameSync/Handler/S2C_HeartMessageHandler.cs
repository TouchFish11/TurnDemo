using Net.FrameSync.Manager;
using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;


namespace Net.FrameSync.Handler
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
