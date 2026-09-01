using System.Threading.Tasks;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动子屏幕接口（嵌套在活动内的可导航界面）
    /// Show/Hide 成对：Show 恢复内容，Hide 清理内容
    /// </summary>
    public interface IActivitySubView
    {
        /// <summary>
        /// 显示
        /// </summary>
        /// <returns></returns>
        Task Show();
        
        /// <summary>
        /// 隐藏
        /// </summary>
        /// <returns></returns>
        Task Hide();
        
        /// <summary>
        /// 销毁
        /// </summary>
        /// <returns></returns>
        Task Destroy();
    }
}
