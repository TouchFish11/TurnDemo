using Core.UI.MVC;

namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// 打开界面事件
    /// </summary>
    public class OpenViewEvent : Event
    {
        /// <summary>
        /// 界面控制器
        /// </summary>
        public IuiController UIController { get; set; }
    }
}
