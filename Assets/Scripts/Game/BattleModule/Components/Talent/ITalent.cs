using GameLogic.BattleMoudule.Entity;
using GameLogic.BattleMoudule.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.BattleMoudule.Talent
{
    /// <summary>
    /// 天赋接口（所有角色天赋统一实现，复用触发逻辑）
    /// </summary>
    public interface ITalent
    {
        string Name { get; }

        IBattleEntity Owner { get; }

        /// <summary>
        /// 触发条件（依赖事件）
        /// </summary>
        /// <param name="battleEvent"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        bool CanTrigger(BattleEvent battleEvent, IBattleEntity owner);

        /// <summary>
        /// 天赋效果执行
        /// </summary>
        /// <param name="battleEvent"></param>
        /// <param name="owner"></param>
        void Execute(BattleEvent battleEvent, IBattleEntity owner);

        /// <summary>
        /// 回合开始时处理
        /// </summary>
        void OnTurnStartHandler(TurnStartEvent turnStartEvent);

        /// <summary>
        /// 回合结束时处理
        /// </summary>
        void OnTurnEndHandler(TurnEndEvent turnEndEvent);
    }
}
