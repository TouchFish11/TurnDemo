using System.Net;
using Net.FrameSync.Manager;
using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.C2S;
using Net.FrameSync.Tcp.Message.S2C;


namespace Net.FrameSync.Handler
{
    /// <summary>
    /// ����ȷ����Ϣ������
    /// </summary>
    public class S2C_ConnectConfirmMessageHandler : MessageHandler<S2C_ConnectConfirmMessage>
    {
        public override S2C_ConnectConfirmMessage TcpMessage {  get; set; }

        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);

            // ���ó�ʼ��ID
            NetManager.Instance.InitClientId(TcpMessage.ClientID);
            //���Ͱ���Ϣ������ˣ�Я������ID��
            NetManager.Instance.SendAsync(new C2S_BindMessage() { ClientID = TcpMessage.ClientID, UdpPort = (NetManager.Instance.udpClientEndPoint as IPEndPoint).Port });
        }
    }
}
