using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Factory;
using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// FireFly���ܹ�����
    /// </summary>
    public class WarriorSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 10:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WarriorNormalSkill(caster, skillId), handler);
                case 11:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WarriorBattleSkill(caster, skillId), handler);
                case 12:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    return new SkillData(new WarriorUltimateSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
