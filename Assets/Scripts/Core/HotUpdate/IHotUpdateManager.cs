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
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        Task LoadAssembliesAsync(string abName);

        /// <summary>
        /// 获取加载的指定热更程序集
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        Assembly GetAssembly(string assemblyName);
        
        /// <summary>
        /// 获取所有加载的热更程序集
        /// </summary>
        /// <returns></returns>
        Assembly[] GetHotAssemblies();

        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        Assembly[] GetAssemblies();

        Assembly GetCoreModule();

        /// <summary>
        /// 加载指定程序集
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assemblyNames"></param>
        Task LoadAssembliesAsync(string abName, params string[] assemblyNames);

        void LoadAssemblyAsyncByFile(params string[] assemblyNames);
    }
}
