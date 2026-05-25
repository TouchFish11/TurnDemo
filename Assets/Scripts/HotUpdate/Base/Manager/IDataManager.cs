using System.Threading.Tasks;

namespace HotUpdate.Base.Manager
{
    /// <summary>
    /// 数据管理器接口
    /// </summary>
    public interface IDataManager
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <returns></returns>
        Task LoadDataAsync();
        
        /// <summary>
        /// 保存数据
        /// </summary>
        void SaveData();
        
        /// <summary>
        /// 异步保存数据
        /// </summary>
        /// <returns></returns>
        Task SaveDataAsync();
    }
}
