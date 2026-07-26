using System.Threading.Tasks;

namespace HotUpdate.Base.Data
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

        /// <summary>
        /// 加载SO配置数据
        /// </summary>
        /// <returns></returns>
        Task LoadConfigAsync();

        /// <summary>
        /// 同步加载数据
        /// </summary>
        void LoadData();
    }
}
