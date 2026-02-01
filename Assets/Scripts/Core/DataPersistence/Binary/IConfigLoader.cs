using System.Threading.Tasks;

namespace Core.DataPersistence.Binary
{
    /// <summary>
    /// 配置加载器接口
    /// </summary>
    public interface IConfigLoader
    {
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        Task LoadConfig();

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetConfig<T>() where T : class;

    }
}
