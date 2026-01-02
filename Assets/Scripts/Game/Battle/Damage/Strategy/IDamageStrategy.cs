using Game.Battle;

/// <summary>
/// 伤害处理策略接口
/// </summary>
public interface IDamageStrategy
{
    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="source">攻击者</param>
    /// <param name="target">防御者</param>
    /// <param name="skillInfo">额外数据</param>
    /// <returns></returns>
    void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);
}
