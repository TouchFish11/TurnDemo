using Net.TCP.Message;

/// <summary>
/// 消息处理器
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
