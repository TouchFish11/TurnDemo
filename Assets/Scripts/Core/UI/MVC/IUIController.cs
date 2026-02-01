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
        /// 销毁
        /// </summary>
        void Destroy();
    }
}
