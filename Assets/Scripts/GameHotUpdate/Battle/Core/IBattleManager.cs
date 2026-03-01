using System;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Turn;

namespace GameHotUpdate.Battle.Core
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
        /// <param name="turnData"></param>
        /// <param name="onBattleOver"></param>
        System.Threading.Tasks.Task EnterBattle(TurnData turnData, Func<System.Threading.Tasks.Task> onBattleOver);

        TurnCreator GetTurnCreator();
    }
}
