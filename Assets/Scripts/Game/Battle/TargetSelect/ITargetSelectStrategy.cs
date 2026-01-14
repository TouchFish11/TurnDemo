using Game.Battle;

/// <summary>
/// 目标选择策略接口
/// </summary>
public interface ITargetSelectStrategy
{
    /// <summary>
    /// 优先级
    /// 越高越先执行
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// 选择主目标
    /// </summary>
    /// <param name="context"></param>
    /// <param name="caster"></param>
    /// <param name="skillInfo"></param>
    /// <returns></returns>
    IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo);
}
