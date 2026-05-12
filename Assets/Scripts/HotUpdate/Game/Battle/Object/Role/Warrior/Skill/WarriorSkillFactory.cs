using Core.DI;
using Core.Reflection;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill
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
                    var handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WarriorNormalSkill(caster, skillId), handler);
                case 11:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new WarriorBattleSkill(caster, skillId), handler);
                case 12:
                    handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    return new SkillData(new WarriorUltimateSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
