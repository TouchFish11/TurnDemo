using Core.DI;
using Core.Reflection;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

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
                    var handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    var wizardNormalSkill = DIContainer.Create<WizardNormalSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(wizardNormalSkill, handler);
                case 21:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    var wizardBattleSkill = DIContainer.Create<WizardBattleSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(wizardBattleSkill, handler);
                case 22:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    var wizardUltimateSkill = DIContainer.Create<WizardUltimateSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(wizardUltimateSkill, handler);
                default:
                    return null;
            }
        }
    }
}
