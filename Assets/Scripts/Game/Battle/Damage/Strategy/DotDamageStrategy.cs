
using Game.Battle;

/// <summary>
/// 持续伤害处理策略类
/// </summary>
public class DotDamageStrategy : IDamageStrategy
{
    public void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, ISkill skill, out DamageResult damageResult)
    {
        damageResult = new DamageResult();
    }
}
