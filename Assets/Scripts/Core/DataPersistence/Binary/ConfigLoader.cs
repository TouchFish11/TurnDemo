using System.Threading.Tasks;

namespace Core.DataPersistence.Binary
{
    /// <summary>
    /// 配置加载器
    /// </summary>
    public abstract class ConfigLoader : IConfigLoader
    {
        public abstract T GetConfig<T>() where T : class;

        public abstract Task LoadConfig();
    }
}
