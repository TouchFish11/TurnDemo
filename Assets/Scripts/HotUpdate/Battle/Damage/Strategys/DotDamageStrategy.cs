using HotUpdate.Battle.Damage.Data;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Enum;

namespace HotUpdate.Battle.Damage.Strategys
{
    /// <summary>
    /// Dot伤害策略
    /// </summary>
    public class DotDamageStrategy : IDamageStrategy
    {
        public void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult)
        {
            damageResult = default;
        }
        
        public void CalcDotDamage(DotDamageCalcData damageCalcData, out DamageResult damageResult)
        {
            damageResult = new DamageResult(
                damageCalcData.source, 
                damageCalcData.target, 
                damageCalcData.Damage, 
                damageCalcData.ElementType, 
                E_DamageType.Dot, 
                false, 
                -1, 
                0);
        }
    }
}
