using Game.Battle;

/// <summary>
/// 伤害计算管理器接口
/// </summary>
public interface IDamageCalcManager
{
    void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="context"></param>
    void Init(IBattleContext context);
}
