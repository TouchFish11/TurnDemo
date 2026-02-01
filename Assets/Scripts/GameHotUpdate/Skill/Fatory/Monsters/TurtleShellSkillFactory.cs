using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Base;
using Game.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Handlers;
using GameHotUpdate.Battle.Skill.Skills.TurtleShell;

namespace GameHotUpdate.Skill.Fatory.Monsters
{
    /// <summary>
    /// TurtleShell���ܹ�����
    /// </summary>
    public class TurtleShellSkillFactory : SkillFactory
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override ISkill CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 102:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    //var strategy = IFactory.GetTypeInstance<IStatusAddStrategyFactory, StatusAddStrategyFactory, TurtleShellSkillStatusStrategy>()
                    return new TurtleShellSkill(caster, skillId, handler, null);
                default:
                    return null;
            }
        }
    }
}
