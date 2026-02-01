using Core.Net.Tcp.Message;

namespace Core.Net.Tcp.Handler
{
    /// <summary>
    /// ��Ϣ������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MessageHandler<T> : IMessageHandler where T : TcpMessage, new()
    {
        public abstract T TcpMessage { get; set; }

        public virtual void HandleMessage(TcpMessage tcpMessage)
        {
            TcpMessage = tcpMessage as T;
        }
    }
}
