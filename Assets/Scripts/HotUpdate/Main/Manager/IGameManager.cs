namespace HotUpdate.Main.Manager
{
    using Task = System.Threading.Tasks.Task;
    
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
        /// 游戏服务管理器
        /// </summary>
        GameServiceManger GameServiceManger { get; }

        /// <summary>
        /// 异步初始化数据
        /// </summary>
        /// <returns></returns>
        Task Init();
    }
}
