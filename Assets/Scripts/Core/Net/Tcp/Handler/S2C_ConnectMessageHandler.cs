using Core.Net.FrameSync.Manager;
using Core.Net.Tcp.Message;
using Core.Net.Tcp.Message.S2C;
using UnityEngine;

namespace Core.Net.Tcp.Handler
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

            // ����
            if (TcpMessage.ConnectState)
            {
                // ����������Ϣ�ķ���
                NetManager.Instance.GetTcpClient().StartSendHeartMsg();
                Debug.Log($"���ӷ������ɹ�");
            }
            // �Ͽ�
            else
            {
                NetManager.Instance.CloseConnect(TcpMessage.ClientID);
            }

            // ������ɺ�
            //DIContainer.GetInstance<IEventCenter>().TriggerEvent(new PostConnectedEvent() { S2C_ConnectMessage = TcpMessage });
        }
    }
}
