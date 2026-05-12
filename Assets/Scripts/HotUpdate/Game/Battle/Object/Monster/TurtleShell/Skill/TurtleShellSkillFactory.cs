using Core.DI;
using Core.Reflection;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.TurtleShell.Skill
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
                    var handler = DIContainer.GetInstance<IFactoryManager>().
                        GetFactory<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>().
                        GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    
                    return new SkillData(new TurtleShellSkill(caster, skillId), handler);
                default:
                    return null;
            }
        }
    }
}
