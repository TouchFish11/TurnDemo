using System.Threading.Tasks;

namespace Core.UI.ViewController
{
    public interface IuiController
    {
        int PanelId { get; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        Task Init(int id, IuiView view);
        
        /// <summary>
        /// 显示
        /// 处理业务逻辑
        /// </summary>
        /// <returns></returns>
        Task Activate();
        
        /// <summary>
        /// 隐藏
        /// 处理业务逻辑
        /// </summary>
        /// <returns></returns>
        Task InActivate();
        
        /// <summary>
        /// 销毁
        /// 主要处理资源释放的逻辑
        /// </summary>
        Task Destroy();
    }
}
