using Framework;
using Game;
using Game.Battle;

/// <summary>
/// Herta技能工厂类
/// </summary>
public class HertaSkillFactory : SkillFactory
{
    public override ISkill CreateSkill(IBattleEntityObject caster, int skillId)
    {
        switch (skillId)
        {
            case 20:
                return new HertaNormalSkill(caster, skillId, IFactory.GetTypeInstance<SkillCastPostHandlerFactory, BaseSkillCastPostHandler>(), null);
            case 21:
                return new HertaBattleSkill(caster, skillId, IFactory.GetTypeInstance<SkillCastPostHandlerFactory, BaseSkillCastPostHandler>(), null);
            case 22:
                return new HertaUltimateSkill(caster, skillId, IFactory.GetTypeInstance<SkillCastPostHandlerFactory, BaseUltimateSkillCastPostHandler>(), null);
            default:
                return null;
        }
    }
}

/// <summary>
/// Herta角色类
/// </summary>
public class Herta : PlayerObject
{
    public override void BattleInit(int roleId, IBattleContext context)
    {
        base.BattleInit(roleId, context);

        // 初始化技能组件
        this.GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new HertaSkillFactory());
    }
}
