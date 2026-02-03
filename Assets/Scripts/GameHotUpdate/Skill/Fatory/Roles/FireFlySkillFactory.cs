using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Handlers;
using GameHotUpdate.Battle.Skill.Skills.FireFly;

namespace GameHotUpdate.Skill.Fatory.Roles
{
    /// <summary>
    /// FireFly���ܹ�����
    /// </summary>
    public class FireFlySkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 10:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new FireFlyNormalSkill(caster, skillId, null), handler);
                case 11:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new FireFlyBattleSkill(caster, skillId, null), handler);
                case 12:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    return new SkillData(new FireFlyUltimateSkill(caster, skillId, null), handler);
                default:
                    return null;
            }
        }
    }
}
