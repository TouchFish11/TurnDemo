using Core.Serialize.Binary;
using Core.Service;
using Game.Battle.Damage;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using GameHotUpdate.Tasks;

namespace GameHotUpdate.Battle.Damage.Strategys
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
            var skillInfo = ServiceLocator.Get<IBinaryDataManager>()
                .GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
            
            damageResult = new DamageResult(attacker, defender, 25, skillInfo.f_elementType.ToElementType(), E_DamageType.Break, false, skillId, resilienceValue);
        }
    }
}
