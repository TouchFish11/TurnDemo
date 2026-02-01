using Core.Net.Tcp.Message;
using Core.Net.Tcp.Message.S2C;
using UnityEngine;

namespace Core.Net.Tcp
{
    /// <summary>
    /// TCP��Ϣ����
    /// </summary>
    public class TcpMessageFactory
    {
        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="bytes"></param>
        /// <param name="nowIndex"></param>
        /// <returns></returns>
        public static TcpMessage CreateMessage(int msgId, byte[] bytes, int nowIndex)
        {
            TcpMessage tcpMessage = null;
            //������Ϣ��
            switch (msgId)
            {
                case 2000:
                    tcpMessage = new S2C_HeartMessage();
                    break;
                case 2001:
                    tcpMessage = new S2C_ConnectMessage();
                    break;
                case 2008:
                    tcpMessage = new S2C_ConnectConfirmMessage();
                    break;
                default:
                    Debug.LogError($"δ�������Ϣ���ͣ�{msgId}");
                    break;
            }

            // ������Ϣ
            tcpMessage.Deserialize(bytes, nowIndex);
            return tcpMessage;
        }
    }
}
