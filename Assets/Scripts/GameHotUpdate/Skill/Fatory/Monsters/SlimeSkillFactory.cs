using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using GameHotUpdate.Battle.Skill.Base;
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
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 101:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    //var strategy = IFactory.GetTypeInstance<IStatusAddStrategyFactory, StatusAddStrategyFactory, TurtleShellSkillStatusStrategy>()
                    return new SkillData(new SlimeSkill(caster, skillId, null), handler);
                default:
                    return null;
            }
        }
    }
}
