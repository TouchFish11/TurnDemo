using Core.UI.ViewController;

namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// 关闭界面事件
    /// </summary>
    public class CloseViewEvent : Event
    {
        /// <summary>
        /// 界面控制器
        /// </summary>
        public IuiController UIController { get; set; }
    }
}
