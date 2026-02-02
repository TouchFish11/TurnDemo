namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// 关闭界面事件
    /// </summary>
    public class CloseViewEvent : Event
    {
        /// <summary>
        /// 界面控制器名称
        /// </summary>
        public string ControllerName { get; set; }
    }
}
