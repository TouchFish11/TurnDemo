using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Factory;
using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Object.Monster.TurtleShell.Skill
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
