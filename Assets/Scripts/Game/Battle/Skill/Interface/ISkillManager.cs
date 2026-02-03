namespace Game.Battle.Skill.Interface
{
    public interface ISkillManager
    {
        /// <summary>
        /// ���Ӽ�������غ϶���
        /// </summary>
        /// <param name="skilldata"></param>
        void AddSkillCommand(ISkillData skilldata);

        /// <summary>
        /// ��ʼ������Ŀ��
        /// ͨ��Ŀ��ѡ���������ʼ������Ŀ��
        /// </summary>
        /// <param name="skill"></param>
        void InitSkillTarget(ISkill skill);
    }
}
