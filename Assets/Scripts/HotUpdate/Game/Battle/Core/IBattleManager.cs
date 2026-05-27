using System;
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
        IBattleContext GetContext();

        /// <summary>
        /// 进入战斗
        /// 唯一入口
        /// </summary>
        /// <param name="turnData">战斗回合数据</param>
        /// <param name="OnPreEnter">战斗进入回调</param>
        /// <param name="onBattleOver">战斗结束回调</param>
        Task EnterBattle(TurnData turnData, Func<Task> OnPreEnter, Func<Task> onBattleOver);

        ITurnCreator GetTurnCreator();
    }
}
