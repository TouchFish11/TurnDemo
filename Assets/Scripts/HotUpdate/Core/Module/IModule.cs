namespace HotUpdate.Core.Module
{
    /// <summary>
    /// 模块接口
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// 异步初始化模块
        /// </summary>
        /// <returns></returns>
        System.Threading.Tasks.Task InitModuleAsync();
    }
}
