using HotUpdate.Base;

using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Damage
{
    /// <summary>
    /// 伤害计算管理器接口
    /// </summary>
    public interface IDamageCalcManager
    {
        /// <summary>
        /// 计算技能伤害
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="skillInfo"></param>
        /// <param name="damageResult"></param>
        void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);

        void Init(IBattleContext context);
    }
}
