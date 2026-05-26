using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Core;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill
{
    /// <summary>
    /// 法师技能工厂
    /// </summary>
    public class WizardSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 20:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var wizardNormalSkill = DIContainer.Create<WizardNormalSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(wizardNormalSkill, handler);
                case 21:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var wizardBattleSkill = DIContainer.Create<WizardBattleSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(wizardBattleSkill, handler);
                case 22:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    var wizardUltimateSkill = DIContainer.Create<WizardUltimateSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(wizardUltimateSkill, handler);
                default:
                    return null;
            }
        }
    }
}
