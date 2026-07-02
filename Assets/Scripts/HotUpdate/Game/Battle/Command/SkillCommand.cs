using System.Collections;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Skill;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 技能指令
    /// </summary>
    public class SkillCommand : Command
    {
        public override int Priority { get; protected set; } = 1;
        
        public ISkill Skill { get; private set; }

        /// <summary>
        /// 初始化指令
        /// </summary>
        /// <param name="skill"></param>
        public void Init(ISkill skill)
        {
            Sender = skill.SkillContext.Caster;
            Skill = skill;
        }
        
        /// <summary>
        /// ִ执行技能
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerator Execute(IBattleContext context)
        {
            yield return Skill.Cast(context);
            Sender.Context.GetEventBus().TriggerEvent(new PostCastEvent(Sender.Context));
        }

        public override IEnumerator ExcutePostProcess(IBattleContext context)
        {
            var skillContext = Skill.SkillContext;
            yield return skillContext.SkillCastPostHandler.Handle(skillContext);
        }
    }
}
