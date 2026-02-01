using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Base;
using Game.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Handlers;
using GameHotUpdate.Battle.Skill.Skills.Herta;

namespace GameHotUpdate.Skill.Fatory.Roles
{
    /// <summary>
    /// Herta���ܹ�����
    /// </summary>
    public class HertaSkillFactory : SkillFactory
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override ISkill CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 20:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    return new HertaNormalSkill(caster, skillId, handler, null);
                case 21:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    return new HertaBattleSkill(caster, skillId, handler, null);
                case 22:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    return new HertaUltimateSkill(caster, skillId, handler, null);
                default:
                    return null;
            }
        }
    }
}
