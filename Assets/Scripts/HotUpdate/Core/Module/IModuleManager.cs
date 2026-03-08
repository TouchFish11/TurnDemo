namespace HotUpdate.Core.Module
{
    using Task = System.Threading.Tasks.Task;

    public interface IModuleManager
    {
        /// <summary>
        /// 初始化所有热更程序集的模块
        /// </summary>
        Task InitModules();
        
        /// <summary>
        /// 获取指定模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetModule<T>() where T : class, IModule;
    }
}
