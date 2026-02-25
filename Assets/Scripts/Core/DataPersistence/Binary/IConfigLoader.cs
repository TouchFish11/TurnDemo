using System;
using System.Threading.Tasks;

namespace Core.DataPersistence.Binary
{
    /// <summary>
    /// 配置加载器接口
    /// </summary>
    public interface IConfigLoader
    {
        /// <summary>
        /// 配置加载事件
        /// </summary>
        event Func<IConfigLoader, Task> OnConfigLoaded;
        
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="ab"></param>
        /// <returns></returns>
        Task LoadConfig(string ab);

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetConfig<T>() where T : class;

        /// <summary>
        /// 异步加载指定配置
        /// 需外部手动调用指定类型的配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        Task LoadConfigAsync<T, K>();
    }
}
