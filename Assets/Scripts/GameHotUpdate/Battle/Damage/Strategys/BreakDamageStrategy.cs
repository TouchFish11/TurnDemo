using Game.Battle.Damage;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using Game.Tasks;

namespace GameHotUpdate.Battle.Damage.Strategys
{
    /// <summary>
    /// �����˺�����������
    /// </summary>
    public class BreakDamageStrategy : IDamageStrategy
    {
        public void CalcDamage(IBattleEntityObject attacker, IBattleEntityObject defender, SkillInfo skillInfo, out DamageResult damageResult)
        {
            damageResult = new DamageResult(attacker, defender, 25, skillInfo.f_elementType.ToElementType(), E_DamageType.Break, false, skillInfo);
        }
    }
}
