using Net.TCP.Message;

/// <summary>
/// 消息处理器接口
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// 处理消息
    /// </summary>
    void HandleMessage(TcpMessage tcpMessage);
}
