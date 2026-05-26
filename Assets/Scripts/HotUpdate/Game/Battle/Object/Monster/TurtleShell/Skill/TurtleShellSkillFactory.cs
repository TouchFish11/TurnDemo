using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Core;

namespace HotUpdate.Game.Battle.Object.Monster.TurtleShell.Skill
{
    /// <summary>
    /// TurtleShell技能工厂
    /// </summary>
    public class TurtleShellSkillFactory : SkillFactory
    {
        public override ISkillData CreateSkill(IBattleEntityObject caster, int skillId)
        {
            switch (skillId)
            {
                case 102:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var turtleShellSkill = DIContainer.Create<TurtleShellSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(turtleShellSkill, handler);
                default:
                    return null;
            }
        }
    }
}
