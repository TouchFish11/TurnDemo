using Core.Reflection;
using Game.Battle.Skill;
using Game.Battle.Toughness;

namespace Game.Battle.Command
{
    public interface ICommandFactory : IFactory
    {
        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        ISkillCommand GetSkillCommand(ISkill skill);

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        IToughnessCommand GetToughnessCommand(IToughnessComponent component);
    }
}
