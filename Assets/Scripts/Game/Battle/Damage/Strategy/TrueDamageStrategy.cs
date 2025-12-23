
using Framework;
using Game.Battle;

/// <summary>
/// 真实伤害处理策略类
/// </summary>
public class TrueDamageStrategy : IDamageStrategy
{
    public void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, ISkill skill, out DamageResult damageResult)
    {
        LogManager.Log("真实伤害策略执行");
        damageResult = new DamageResult();
    }
}
