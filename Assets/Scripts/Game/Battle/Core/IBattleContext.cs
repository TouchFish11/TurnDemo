using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Battle
{
    public interface IBattleContext
    {
        /// <summary>
        /// 获取所有战斗的实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBattleEntityObject> GetAllBattleEntity();

        /// <summary>
        /// 获取回合管理器
        /// </summary>
        /// <returns></returns>
        TurnManager GetTurnManager();

        /// <summary>
        /// 初始化战斗
        /// </summary>
        Task InitBattle();

        /// <summary>
        /// 清理战斗
        /// </summary>
        void CleanupBattle();

        /// <summary>
        /// 获取战斗事件总线
        /// </summary>
        /// <returns></returns>
        BattleEventBus GetEventBus();
    }
}
