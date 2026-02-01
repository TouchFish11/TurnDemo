using Game.Tasks;

namespace Game.Manager
{
    /// <summary>
    /// 游戏管理器接口
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// 初始化游戏相关服务
        /// </summary>
        void InitGameService();

        /// <summary>
        /// 任务数据集合
        /// </summary>
        TaskDataCollection TaskDataCollection { get; }
    }
}
