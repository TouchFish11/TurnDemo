using Framework;

/// <summary>
/// 全局消息事件
/// </summary>
public class GlobalMessageEvent : IEvent
{
    public string Message { get; }

    public GlobalMessageEvent(string message)
    {
        Message = message;
    }

    void IEvent.ResetEvent()
    {

    }
}
