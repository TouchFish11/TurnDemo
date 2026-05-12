using System.Collections.Generic;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Skill;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// ���ܹ���
    /// </summary>
    public abstract class SkillFactory : ISkillFactory
    {
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
