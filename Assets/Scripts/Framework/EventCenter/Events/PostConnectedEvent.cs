using Framework;
using Net.TCP.Message.S2C;

/// <summary>
/// 连接完成后事件
/// </summary>
public class PostConnectedEvent : Event
{
    public S2C_ConnectMessage S2C_ConnectMessage { get; set; }
}
