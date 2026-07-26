using System.Threading.Tasks;

namespace HotUpdate.Base.Data
{
    /// <summary>
    /// 数据持久化接口
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// 加载数据
        /// </summary>
        void LoadData();
        
        /// <summary>
        /// 保存数据
        /// </summary>
        void SaveData();
        
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
}
