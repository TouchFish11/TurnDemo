namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// 光标可见性变化事件
    /// </summary>
    public class MouseVisibleChangedEvent : Event
    {
        /// <summary>
        /// 来源
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 是否显示/隐藏光标
        /// </summary>
        public bool IsVisible { get; set; }

    }
}
