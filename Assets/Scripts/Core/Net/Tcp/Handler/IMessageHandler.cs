using Core.Net.Tcp.Message;

namespace Core.Net.Tcp.Handler
{
    /// <summary>
    /// ��Ϣ�������ӿ�
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// ������Ϣ
        /// </summary>
        void HandleMessage(TcpMessage tcpMessage);
    }
}
