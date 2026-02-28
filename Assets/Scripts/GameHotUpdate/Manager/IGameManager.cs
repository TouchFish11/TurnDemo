using System.Threading.Tasks;

namespace GameHotUpdate.Manager
{
    /// <summary>
    /// 游戏管理器接口
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// 游戏数据管理器
        /// </summary>
        IGameDataManager GameDataManager { get; }
        
        /// <summary>
        /// 游戏服务管理器
        /// </summary>
        IGameServiceManger  GameServiceManger { get; }

        /// <summary>
        /// 异步初始化数据
        /// </summary>
        /// <param name="gameDataManager"></param>
        /// <param name="gameServiceManger"></param>
        /// <returns></returns>
        Task Init(IGameDataManager gameDataManager, IGameServiceManger gameServiceManger);
    }
}
