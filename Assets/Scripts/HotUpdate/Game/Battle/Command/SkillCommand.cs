using System;
using System.Collections;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.Skill;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 技能指令
    /// </summary>
    public class SkillCommand : Command
    {
        public override IBattleEntityObject Sender { get; protected set; }
        public override ECommandPriority Priority { get; protected set; }
        
        public ISkill Skill { get; private set; }

        /// <summary>
        /// 初始化指令
        /// </summary>
        /// <param name="skill"></param>
        public void Init(ISkill skill)
        {
            Sender = skill.SkillContext.Caster;

            // TODO:暂时这样处理，应该配置
            //Priority = skill.SkillContext.SkillInfo.Priority;
            Priority = (E_SkillType)skill.SkillContext.SkillInfo.f_SkillType switch
            {
                E_SkillType.Monster => ECommandPriority.MonsterSkill,
                E_SkillType.UltimateSkill => ECommandPriority.Ultimate,
                E_SkillType.NormalAttack 
                    or E_SkillType.CombatSkill
                    or E_SkillType.EnhancedNormalAttack 
                    or E_SkillType.EnhancedCombatSkill => ECommandPriority.RoleSkill,
                _ => throw new ArgumentOutOfRangeException()
            };

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
            
            // 角色释放技能才需要触发事件，怪物不需要
            if(Sender is IRoleObject)
                Sender.Context.EventBus.TriggerEvent(new PostCastEvent(Sender.Context));
        }

        public override IEnumerator ExcutePostProcess(IBattleContext context)
        {
            var skillContext = Skill.SkillContext;
            yield return skillContext.SkillCastPostHandler.Handle(skillContext);
        }
    }
}
