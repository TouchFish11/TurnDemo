using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Damage.Data;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.Damage.Strategys
{
    /// <summary>
    /// 击破伤害策略
    /// </summary>
    public class BreakDamageStrategy : IDamageStrategy
    {
        public void CalcDamage(IBattleEntityObject attacker, IBattleEntityObject defender, SkillInfo skillInfo, out DamageResult damageResult)
        {
            damageResult = default;
        }

        public void CalcBreakDamage(IBattleEntityObject attacker, IBattleEntityObject defender, int skillId, int resilienceValue, out DamageResult damageResult)
        {
            var skillInfo = DIContainer.GetInstance<IBinaryDataManager>()
                .GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
            
            damageResult = new DamageResult(attacker, defender, 25, skillInfo.f_elementType.ToElementType(), E_DamageType.Break, false, skillId, resilienceValue);
        }
    }
}
