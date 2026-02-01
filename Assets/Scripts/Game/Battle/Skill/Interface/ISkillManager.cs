namespace Game.Battle.Skill.Interface
{
    public interface ISkillManager
    {
        /// <summary>
        /// ���Ӽ�������غ϶���
        /// </summary>
        /// <param name="skill"></param>
        void AddSkillCommand(ISkill skill);

        /// <summary>
        /// ��ʼ������Ŀ��
        /// ͨ��Ŀ��ѡ���������ʼ������Ŀ��
        /// </summary>
        /// <param name="skill"></param>
        void InitSkillTarget(ISkill skill);
    }
}
