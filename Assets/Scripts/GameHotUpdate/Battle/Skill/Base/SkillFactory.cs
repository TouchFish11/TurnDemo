using System.Collections.Generic;
using Game.Battle.Objects;
using Game.Battle.Skill.Interface;

namespace GameHotUpdate.Battle.Skill.Base
{
    /// <summary>
    /// ���ܹ���
    /// </summary>
    public abstract class SkillFactory : ISkillFactory
    {
        // /// <summary>
        // /// �����������ܶ���
        // /// </summary>
        // /// <param name="caster"></param>
        // /// <param name="skillIds"></param>
        // /// <returns></returns>
        // public IEnumerable<ISkill> CreateSkills(IBattleEntityObject caster, params int[] skillIds)
        // {
        //     List<ISkill> skills = new List<ISkill>();
        //     foreach (int skillId in skillIds)
        //     {
        //         skills.Add(CreateSkill(caster, skillId));
        //     }
        //     return skills;
        // }

        public IEnumerable<ISkillData> CreateSkills(IBattleEntityObject caster, params int[] skillIds)
        {
            foreach (var skillId in skillIds)
            {
                yield return CreateSkill(caster, skillId);
            }
        }

        /// <summary>
        /// �������ܶ���
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public abstract ISkillData CreateSkill(IBattleEntityObject caster, int skillId);
    }
}
