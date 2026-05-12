using HotUpdate.Base.Battle.Damage;
using HotUpdate.Base.Battle.Damage.Data;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Damage.Strategys
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
