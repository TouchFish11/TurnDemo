using System.Threading.Tasks;

namespace HotUpdate.Base.Data
{
    /// <summary>
    /// 数据管理器接口
    /// </summary>
    public interface IDataManager
    {
        Task LoadDataAsync();
        
        void SaveData();
        
        Task SaveDataAsync();
    }
}
