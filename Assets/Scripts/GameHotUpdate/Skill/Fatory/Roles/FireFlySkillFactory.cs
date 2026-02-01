using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Base;
using Game.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Handlers;
using GameHotUpdate.Battle.Skill.Skills.FireFly;

namespace GameHotUpdate.Skill.Fatory.Roles
{
    /// <summary>
    /// FireFly���ܹ�����
    /// </summary>
    public class FireFlySkillFactory : SkillFactory
    {
        public override ISkill CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 10:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    return new FireFlyNormalSkill(caster, skillId, handler, null);
                case 11:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    return new FireFlyBattleSkill(caster, skillId, handler, null);
                case 12:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    return new FireFlyUltimateSkill(caster, skillId, handler, null);
                default:
                    return null;
            }
        }
    }
}
