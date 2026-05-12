using Core.DI;
using Core.Reflection;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill
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
                    var handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WizardNormalSkill(caster, skillId), handler);
                case 21:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WizardBattleSkill(caster, skillId), handler);
                case 22:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    return new SkillData(new WizardUltimateSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
