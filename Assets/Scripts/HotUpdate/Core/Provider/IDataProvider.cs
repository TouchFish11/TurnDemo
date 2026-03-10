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
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <returns></returns>
        Task LoadDataAsync();
        
        /// <summary>
        /// 异步保存数据
        /// </summary>
        /// <returns></returns>
        Task SaveDataAsync();
    }
    
    /// <summary>
    /// 数据提供器接口
    /// </summary>
    public interface IDataProvider<out T> : IDataProvider
    {
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        T GetData();
    }
}
