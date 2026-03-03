using System.Threading.Tasks;

namespace Core.UI.MVC
{
    public interface IuiController
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="view"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task Init(IuiView view, IuiModel model);
        
        /// <summary>
        /// 显示
        /// 处理业务逻辑
        /// </summary>
        /// <returns></returns>
        Task Show();
        
        /// <summary>
        /// 隐藏
        /// 处理业务逻辑
        /// </summary>
        /// <returns></returns>
        Task Hide();
        
        /// <summary>
        /// 销毁
        /// 主要处理资源释放的逻辑
        /// </summary>
        Task Destroy();
    }
}
