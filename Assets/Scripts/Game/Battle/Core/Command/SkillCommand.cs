using System.Collections;

namespace Game.Battle
{
    /// <summary>
    /// 技能命令
    /// 封装技能的调用
    /// </summary>
    public class SkillCommand : Command
    {
        /// <summary>
        /// 技能对象
        /// </summary>
        public ISkill Skill { get; private set; }

        public override int Priority { get; protected set; }

        /// <summary>
        /// 执行技能
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerator Excute(IBattleContext context)
        {
            return Skill.Cast(context);
        }

        /// <summary>
        /// 初始化技能命令
        /// </summary>
        /// <param name="skill"></param>
        public void Init(ISkill skill)
        {
            this.Sender = skill.Caster;
            this.Skill = skill;
        }
    }
}
