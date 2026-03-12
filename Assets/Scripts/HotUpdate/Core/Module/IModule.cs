namespace HotUpdate.Core.Module
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 模块接口
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 初始化优先级
        /// 数值越小越先初始化
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// 异步初始化模块
        /// </summary>
        /// <returns></returns>
        Task InitModuleAsync();
    }
}
