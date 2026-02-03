using Core.Reflection;
using Game.Battle.Skill;
using Game.Battle.Skill.Interface;
using Game.Battle.Toughness;

namespace Game.Battle.Command
{
    public interface ICommandFactory : IFactory
    {
        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="skillData"></param>
        /// <returns></returns>
        ISkillCommand GetSkillCommand(ISkillData skillData);

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        IToughnessCommand GetToughnessCommand(IToughnessComponent component);
    }
}
