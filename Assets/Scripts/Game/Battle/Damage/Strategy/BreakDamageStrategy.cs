using Framework;
using Game.Battle;

/// <summary>
/// 击破伤害处理策略类
/// </summary>
public class BreakDamageStrategy : IDamageStrategy
{
    public void CalcDamage(IBattleEntityObject attacker, IBattleEntityObject defender, ISkill skill, out DamageResult damageResult)
    {
        LogManager.Log("击破伤害处理逻辑执行");
        damageResult = new DamageResult();
    }
}
