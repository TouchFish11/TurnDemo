using Framework;
using Net.TCP.Message.S2C;

/// <summary>
/// 连接完成后事件
/// </summary>
public class PostConnectedEvent : IEvent
{
    public S2C_ConnectMessage S2C_ConnectMessage { get; }

    public PostConnectedEvent(S2C_ConnectMessage s2C_ConnectMessage)
    {
        S2C_ConnectMessage = s2C_ConnectMessage;
    }

    void IEvent.ResetEvent()
    {

    }
}
