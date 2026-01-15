using Framework;
using Game.Battle;

/// <summary>
/// 击破伤害处理策略类
/// </summary>
public class BreakDamageStrategy : IDamageStrategy
{
    public void CalcDamage(IBattleEntityObject attacker, IBattleEntityObject defender, SkillInfo skillInfo, out DamageResult damageResult)
    {
        damageResult = new DamageResult(attacker, defender, 25, skillInfo.f_elementType.ToElementType(), E_DamageType.Break, false, skillInfo);
    }
}
