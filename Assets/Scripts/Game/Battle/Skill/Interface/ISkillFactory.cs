using System.Collections.Generic;
using Game.Battle.Objects;

namespace Game.Battle.Skill.Interface
{
    /// <summary>
    /// ���ܹ����ӿ�
    /// </summary>
    public interface ISkillFactory
    {
        /// <summary>
        /// ��������ʵ��
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        ISkill CreateSkill(IBattleEntityObject caster, int skillId);
    
        /// <summary>
        /// ������������ʵ��
        /// </summary>
        /// <param name="skillIds"></param>
        /// <returns></returns>
        IEnumerable<ISkill> CreateSkills(IBattleEntityObject caster, params int[] skillIds);
    }
}
