using System;
using System.Threading.Tasks;

namespace Core.Serialize.Binary
{
    /// <summary>
    /// 二进制数据管理器接口
    /// </summary>
    public interface IBinaryDataManager
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T">配置容器类型</typeparam>
        /// <param name="loadType"></param>
        /// <returns></returns>
        T GetConfig<T>(EConfigLoadType loadType) where T : class;

        /// <summary>
        /// 异步加载本地文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<T> LoadAsync<T>(string fileName) where T : new();

        /// <summary>
        /// 异步加载配置
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        Task LoadConfigAsync(string abName);

        /// <summary>
        /// 异步保存文件到本地
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        Task SaveAsync(string fileName, object obj);

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="loadType"></param>
        /// <param name="onConfigLoaded"></param>
        void AddConfig(EConfigLoadType loadType, Func<IConfigLoader, Task> onConfigLoaded);
    }
}
