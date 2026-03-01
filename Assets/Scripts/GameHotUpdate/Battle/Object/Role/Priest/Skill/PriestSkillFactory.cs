using Core.Reflection;
using Core.Service;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Factory;
using GameHotUpdate.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Interface;

namespace GameHotUpdate.Battle.Object.Role.Priest.Skill
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
