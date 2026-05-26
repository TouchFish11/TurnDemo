using Core.DI;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill
{
    /// <summary>
    /// 牧师技能工厂
    /// </summary>
    public class PriestSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 30:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var priestNormalSkill = DIContainer.Create<PriestNormalSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(priestNormalSkill, handler);
                case 31:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var priestBattleSkill = DIContainer.Create<PriestBattleSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(priestBattleSkill, handler);
                case 32:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    var priestUltimateSkill = DIContainer.Create<PriestUltimateSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(priestUltimateSkill, handler);
                default:
                    return null;
            }
        }
    }
}
