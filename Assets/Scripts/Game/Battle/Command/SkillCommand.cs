using System.Collections;
using Game.Battle.Context;
using Game.Battle.Skill;

namespace Game.Battle.Command
{
    /// <summary>
    /// ��������
    /// ��װ���ܵĵ���
    /// </summary>
    public class SkillCommand : Command, ISkillCommand
    {
        /// <summary>
        /// ���ܶ���
        /// </summary>
        public ISkill Skill { get; private set; }

        public override int Priority { get; protected set; }

        /// <summary>
        /// ִ�м���
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerator Execute(IBattleContext context)
        {
            return Skill.Cast(context);
        }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="skill"></param>
        public void Init(ISkill skill)
        {
            Sender = skill.Caster;
            Skill = skill;
        }
    }
}
