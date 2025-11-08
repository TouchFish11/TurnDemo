using GameLogic.BattleMoudule.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Core
{
    public interface IBattleContext
    {
        /// <summary>
        /// 获取所有战斗的实体
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBattleEntity> GetAllBattleEntity();

        /// <summary>
        /// 获取回合管理器
        /// </summary>
        /// <returns></returns>
        TurnManager GetTurnManager();

        /// <summary>
        /// 初始化战斗
        /// </summary>
        void InitBattle();
    }
}
