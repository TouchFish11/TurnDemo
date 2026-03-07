namespace HotUpdate.Core.Module
{
    public interface IModuleManager
    {
        /// <summary>
        /// 初始化所有热更程序集的模块
        /// </summary>
        void InitModules();
        
        /// <summary>
        /// 获取指定模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetModule<T>() where T : class, IModule;
    }
}
