using Game.Battle.Skill;

namespace Game.Battle.Command
{
    public interface ISkillCommand : ICommand
    {
        /// <summary>
        /// ���ܶ���
        /// </summary>
        ISkill Skill { get; }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="skill"></param>
        void Init(ISkill skill);
    }
}
