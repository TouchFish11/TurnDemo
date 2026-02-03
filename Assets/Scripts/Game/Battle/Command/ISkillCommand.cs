using Game.Battle.Skill;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;

namespace Game.Battle.Command
{
    public interface ISkillCommand : ICommand
    {
        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="SkillData"></param>
        void Init(ISkillData SkillData);
    }
}
