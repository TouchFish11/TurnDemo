using System.Collections;
using HotUpdate.Game.Battle.Event.UI;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 玩家角色非终结技技能执行效果
    /// </summary>
    public abstract class NonUltimateSkillExecuteNode : SkillExecuteNode
    {
        protected NonUltimateSkillExecuteNode(ISkill skill) : base(skill)
        {
        
        }

        public sealed override IEnumerator Execute()
        {
            var context = skill.SkillContext.Caster.Context;
            context.GetEventBus().TriggerEvent(new PlayerReleaseSkillEvent(context));
            yield return OnExecute();
        }

        protected abstract IEnumerator OnExecute();
    }
}
