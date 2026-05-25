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
        public override int Priority { get; protected set; }
        
        public ISkillData SkillData { get; protected set; }

        /// <summary>
        /// 初始化指令
        /// </summary>
        /// <param name="skillData"></param>
        public void Init(ISkillData skillData)
        {
            Sender = skillData.Skill.Caster;
            SkillData = skillData;
        }
        
        /// <summary>
        /// ִ执行技能
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerator Execute(IBattleContext context)
        {
            yield return SkillData.Skill.Cast(context);
            Sender.Context.GetEventBus().TriggerEvent(new PostCastEvent(Sender.Context));
        }

        public override IEnumerator ExcutePostProcess(IBattleContext context)
        {
            yield return SkillData.SkillCastPostHandler.Handle(SkillData.Skill);
        }
    }
}
