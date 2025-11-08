using GameLogic.BattleMoudule.Core;
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace GameLogic.BattleMoudule.Skill
{
    /// <summary>
    /// 技能接口
    /// </summary>
    public interface ISkill
    {
        string Name { get; }

        /// <summary>
        /// 伤害系数（配置表读取）
        /// </summary>
        float DamageCoefficient { get; }

        /// <summary>
        /// 技能属性（配置表读取）
        /// </summary>
        E_PropertyType PropertyType { get; }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="context"></param>
        /// <param name="caster"></param>
        /// <param name="targets"></param>
        void Cast(IBattleContext context, IBattleEntity caster, List<IBattleEntity> targets); 
    }
}
