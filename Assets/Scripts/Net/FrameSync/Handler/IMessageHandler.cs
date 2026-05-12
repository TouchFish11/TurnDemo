using Net.FrameSync.Tcp.Message;

namespace Net.FrameSync.Handler
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
