using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 应用伤害事件
    /// 伤害管理器计算完伤害后主动触发
    /// </summary>
    public class ApplyDamageEvent : BattleEvent
    {
        /// <summary>
        /// 伤害结果
        /// </summary>
        public DamageResult DamageResult {  get; private set; } 

        public ApplyDamageEvent(IBattleContext context, DamageResult damageResult) : base(context)
        {
            DamageResult = damageResult;
        }
    }
}
