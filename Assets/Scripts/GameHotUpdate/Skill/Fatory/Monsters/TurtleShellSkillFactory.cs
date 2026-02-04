using Core.Reflection;
using Core.Service;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using GameHotUpdate.Battle.Skill.Base;
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
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 102:
                    var handler = ServiceLocator.Get<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new TurtleShellSkill(caster, skillId, null), handler);
                default:
                    return null;
            }
        }
    }
}
