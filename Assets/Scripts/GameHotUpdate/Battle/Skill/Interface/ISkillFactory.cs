using System.Collections.Generic;
using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Battle.Skill.Interface
{
    /// <summary>
    /// ���ܹ����ӿ�
    /// </summary>
    public interface ISkillFactory
    {
        // /// <summary>
        // /// ��������ʵ��
        // /// </summary>
        // /// <param name="caster"></param>
        // /// <param name="skillId"></param>
        // /// <returns></returns>
        // ISkill CreateSkill(IBattleEntityObject caster, int skillId);

        // /// <summary>
        // /// ������������ʵ��
        // /// </summary>
        // /// <param name="caster"></param>
        // /// <param name="skillIds"></param>
        // /// <returns></returns>
        // IEnumerable<ISkill> CreateSkills(IBattleEntityObject caster, params int[] skillIds);

        /// <summary>
        /// ��������ʵ��
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        ISkillData CreateSkill(IBattleEntityObject caster, int skillId);
        
        /// <summary>
        /// ������������ʵ��
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillIds"></param>
        /// <returns></returns>
        IEnumerable<ISkillData> CreateSkills(IBattleEntityObject caster, params int[] skillIds);
    }
}
