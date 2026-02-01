using System.Collections.Generic;
using Game.Battle.Objects;
using Game.Battle.Skill.Interface;

namespace Game.Battle.Skill.Base
{
    /// <summary>
    /// ���ܹ���
    /// </summary>
    public abstract class SkillFactory : ISkillFactory
    {
        /// <summary>
        /// �����������ܶ���
        /// </summary>
        /// <param name="skillIds"></param>
        /// <returns></returns>
        public IEnumerable<ISkill> CreateSkills(IBattleEntityObject caster, params int[] skillIds)
        {
            List<ISkill> skills = new List<ISkill>();
            foreach (int skillId in skillIds)
            {
                skills.Add(CreateSkill(caster, skillId));
            }
            return skills;
        }

        /// <summary>
        /// �������ܶ���
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public abstract ISkill CreateSkill(IBattleEntityObject caster, int skillId);
    }
}
