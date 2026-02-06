using Game.Battle.Skill.Interface;

namespace Game.Battle.Command
{
    public interface ISkillCommand : ICommand
    {
        /// <summary>
        /// 技能数据
        /// </summary>
        ISkillData SkillData { get; }
        
        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="SkillData"></param>
        void Init(ISkillData SkillData);
    }
}
