using System.Collections.Generic;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能工厂
    /// </summary>
    public abstract class SkillFactory : ISkillFactory
    {
        [Inject] protected ISkillCastPostHandlerFactory skillCastPostHandlerFactory;
        
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
