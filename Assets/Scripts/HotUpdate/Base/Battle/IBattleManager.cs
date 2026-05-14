using System;
using HotUpdate.Base.Battle.Turn;

namespace HotUpdate.Base.Battle
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
        /// <param name="OnPreEnter"></param>
        /// <param name="onBattleOver">战斗结束回调</param>
        System.Threading.Tasks.Task EnterBattle(TurnData turnData, Func<System.Threading.Tasks.Task> OnPreEnter,
            Func<System.Threading.Tasks.Task> onBattleOver);

        ITurnCreator GetTurnCreator();
    }
}
