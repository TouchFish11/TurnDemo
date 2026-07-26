using System.Threading.Tasks;

namespace HotUpdate.Base.Data
{
    /// <summary>
    /// SO配置来源接口
    /// </summary>
    public interface ISOConfigSources
    {
        /// <summary>
        /// 异步加载SO配置
        /// </summary>
        /// <returns></returns>
        Task LoadConfigAsync();
    }
}
