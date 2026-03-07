using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Factory;
using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Object.Role.Priest.Skill
{
    public class PriestSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 30:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new PriestNormalSkill(caster, skillId), handler);
                case 31:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new PriestBattleSkill(caster, skillId), handler);
                case 32:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    return new SkillData(new PriestUltimateSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
