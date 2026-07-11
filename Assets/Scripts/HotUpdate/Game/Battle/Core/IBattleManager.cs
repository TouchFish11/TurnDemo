using System.Threading.Tasks;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Turn;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗管理器接口
    /// </summary>
    public interface IBattleManager
    {
        /// <summary>
        /// 战斗波次创建器
        /// </summary>
        WaveCreator WaveCreator { get; }
        
        /// <summary>
        /// 战斗服务对象
        /// </summary>
        BattleService BattleService { get; }
        
        Task Init(IBattleContext context, BattleStartupParams startupParams);

        /// <summary>
        /// 清理战斗数据缓存
        /// </summary>
        void Reset();

        void QuitBattle(int battlePanelId);
    }
}
