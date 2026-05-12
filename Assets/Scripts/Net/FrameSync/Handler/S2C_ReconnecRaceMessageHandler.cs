using Net.FrameSync.Manager;
using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;
using UnityEngine;

namespace Net.FrameSync.Handler
{
    /// <summary>
    /// ����������Ϣ������
    /// </summary>
    public class S2C_ReconnecRaceMessageHandler : MessageHandler<S2C_ReconnecRaceMessage>
    {
        public override S2C_ReconnecRaceMessage TcpMessage { get; set; }

        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);

            if (TcpMessage.ClientID == NetManager.Instance.ClientID)
            {
                //GameHandler.Instance.IsStop = false;
            }
            Debug.Log($"��ң�{TcpMessage.ClientID}����������");
        }
    }
}
