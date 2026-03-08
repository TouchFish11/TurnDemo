using Core.Service;

namespace HotUpdate.Core.Module
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 模块接口
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 异步初始化模块
        /// </summary>
        /// <returns></returns>
        Task InitModuleAsync();
    }
}
