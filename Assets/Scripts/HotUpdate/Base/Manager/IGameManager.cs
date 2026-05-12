namespace HotUpdate.Base.Manager
{
    /// <summary>
    /// 游戏管理器接口
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// 游戏数据管理器
        /// </summary>
        GameDataManager GameDataManager { get; }

        /// <summary>
        /// 异步初始化数据
        /// </summary>
        /// <returns></returns>
        System.Threading.Tasks.Task InitDataAsync();
    }
}
