using System;
using System.Threading.Tasks;
using Game.Battle.Context;
using GameHotUpdate.Battle.Turn;

namespace GameHotUpdate.Battle
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
        Task EnterBattle(TurnData turnData, Func<Task> onBattleOver);

        TurnCreator GetTurnCreator();
    }
}
