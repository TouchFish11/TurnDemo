namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// 全局消息事件
    /// </summary>
    public class GlobalMessageEvent : Event
    {
        /// <summary>
        /// 消息文本
        /// </summary>
        public string Message { get; set; }
    }
}
