using System.Collections;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Skill.Interface;
using GameHotUpdate.Battle.Event.Skill;

namespace GameHotUpdate.Battle.Command
{
    /// <summary>
    /// 技能指令
    /// </summary>
    public class SkillCommand : Game.Battle.Command.Command, ISkillCommand
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
            this.SkillData = skillData;
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
