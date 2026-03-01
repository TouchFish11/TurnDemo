using Core.Reflection;
using Core.Service;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Factory;
using GameHotUpdate.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Interface;

namespace GameHotUpdate.Battle.Object.Role.Wizard.Skill
{
    /// <summary>
    /// Herta���ܹ�����
    /// </summary>
    public class WizardSkillFactory : SkillFactory
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 20:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WizardNormalSkill(caster, skillId), handler);
                case 21:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WizardBattleSkill(caster, skillId), handler);
                case 22:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    return new SkillData(new WizardUltimateSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
