namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// 全局消息事件
    /// </summary>
    public class GlobalMessageEvent : Event
    {
        public string Message { get; set; }
    }
}
