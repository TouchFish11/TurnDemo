using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Base;
using Game.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Handlers;
using GameHotUpdate.Battle.Skill.Skills.Slime;

namespace GameHotUpdate.Skill.Fatory.Monsters
{
    /// <summary>
    /// Slime���ܹ�����
    /// </summary>
    public class SlimeSkillFactory : SkillFactory
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override ISkill CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 101:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    //var strategy = IFactory.GetTypeInstance<IStatusAddStrategyFactory, StatusAddStrategyFactory, TurtleShellSkillStatusStrategy>()
                    return new SlimeSkill(caster, skillId, handler, null);
                default:
                    return null;
            }
        }
    }
}
