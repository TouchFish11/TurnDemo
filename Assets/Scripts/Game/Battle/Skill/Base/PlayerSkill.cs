using Game.Battle;

/// <summary>
/// 玩家技能
/// 玩家角色技能继承
/// </summary>
public abstract class PlayerSkill : Skill
{
    public PlayerSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
    {

    }

    /// <summary>
    /// 玩家技能释放前执行
    /// 处理战技点、更新UI相关逻辑
    /// </summary>
    /// <param name="context"></param>
    protected override void OnPreCast(IBattleContext context)
    {
        base.OnPreCast(context);
        // 处理战技点
        context.ConsumeSkillPoint(SkillInfo.f_costBP);
        // 隐藏UI、更新相关UI
        context.GetEventBus().TriggerEvent(new PlayerReleaseSkillEvent(context));
    }
}
