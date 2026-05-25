using System.Collections.Generic;
using HotUpdate.Base;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能工厂
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
        /// 创建技能
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public abstract ISkillData CreateSkill(IBattleEntityObject caster, int skillId);
    }
}
