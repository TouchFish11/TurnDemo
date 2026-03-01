using Core.Reflection;
using Core.Service;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Factory;
using GameHotUpdate.Battle.Skill.Handler;
using GameHotUpdate.Battle.Skill.Interface;

namespace GameHotUpdate.Battle.Object.Monster.TurtleShell.Skill
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
                    
                    return new SkillData(new TurtleShellSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
