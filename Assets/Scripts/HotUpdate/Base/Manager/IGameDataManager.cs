using System.Threading.Tasks;

namespace HotUpdate.Base.Manager
{
    public interface IGameDataManager
    {
        /// <summary>
        /// 异步加载数据
        /// </summary>
        Task LoadDataAsync();

        /// <summary>
        /// 异步保存数据
        /// </summary>
        Task SaveDataAsync();

        /// <summary>
        /// 同步保存数据
        /// </summary>
        void SaveData();
    }
}
