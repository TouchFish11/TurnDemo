using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Damage.Data;
using HotUpdate.Game.Battle.Object;

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
