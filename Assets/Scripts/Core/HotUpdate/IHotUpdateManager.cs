using System.Reflection;
using System.Threading.Tasks;

namespace Core.HotUpdate
{
    /// <summary>
    /// 热更新管理器接口
    /// </summary>
    public interface IHotUpdateManager
    {
        /// <summary>
        /// 加载所有热更程序集
        /// 加载后会覆盖原来的缓存
        /// </summary>
        /// <returns></returns>
        Task LoadAssemblys();
        
        /// <summary>
        /// 加载指定程序集
        /// 加载后会覆盖原来的缓存
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        Task LoadAssembly(string assemblyName);

        /// <summary>
        /// 获取加载的指定程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        Assembly GetAssembly(string assemblyName);
        
        /// <summary>
        /// 获取所有加载的热更程序集
        /// </summary>
        /// <returns></returns>
        Assembly[] GetAssemblies();
        
        /// <summary>
        /// 卸载指定程序集
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        void UnloadAssembly(string assemblyName);

        /// <summary>
        /// 卸载所有程序集
        /// </summary>
        void UnloadAll();
    }
}
