namespace HotUpdate.Base.Provider
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
        
        /// <summary>
        /// 同步保存数据
        /// </summary>
        void SaveData();
    }
}
