using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Factory;
using GameHotUpdate.Battle.Skill.Handlers;

namespace GameHotUpdate.Battle.Object.Monster.Slime.Skill
{
    /// <summary>
    /// Slime���ܹ�����
    /// </summary>
    public class SlimeSkillFactory : SkillFactory
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 101:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new SlimeSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
