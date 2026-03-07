using System.Threading.Tasks;
using HotUpdate.Core.Data;

namespace HotUpdate.Core.Provider
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 数据提供器接口
    /// </summary>
    public interface IDataProvider
    {
        Task SaveDataAsync();
    }
    
    /// <summary>
    /// 数据提供器接口
    /// </summary>
    public interface IDataProvider<T> : IDataProvider
    {
        Task<T> GetDataAsync();
    }
}
