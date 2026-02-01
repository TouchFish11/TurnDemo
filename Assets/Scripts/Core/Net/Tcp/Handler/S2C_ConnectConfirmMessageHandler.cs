using System.Net;
using Core.Net.FrameSync.Manager;
using Core.Net.Tcp.Message;
using Core.Net.Tcp.Message.C2S;
using Core.Net.Tcp.Message.S2C;

namespace Core.Net.Tcp.Handler
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
            // ���Ͱ���Ϣ������ˣ�Я������ID��
            NetManager.Instance.GetTcpClient().EnqueueMessage(new C2S_BindMessage() { ClientID = TcpMessage.ClientID, UdpPort = (NetManager.Instance.udpClientEndPoint as IPEndPoint).Port });
        }
    }
}
