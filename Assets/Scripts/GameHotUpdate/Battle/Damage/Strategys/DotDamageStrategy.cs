using Game.Battle.Damage;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Damage.Strategys
{
    /// <summary>
    /// �����˺�����������
    /// </summary>
    public class DotDamageStrategy : IDamageStrategy
    {
        public void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult)
        {
            damageResult = new DamageResult();
        }
    }
}
