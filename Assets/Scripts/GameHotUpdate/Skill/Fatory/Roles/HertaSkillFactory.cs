using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using GameHotUpdate.Battle.Skill.Base;
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
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 20:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new HertaNormalSkill(caster, skillId, null), handler);
                case 21:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new HertaBattleSkill(caster, skillId, null), handler);
                case 22:
                    handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    
                    return new SkillData(new HertaUltimateSkill(caster, skillId, null), handler);
                default:
                    return null;
            }
        }
    }
}
