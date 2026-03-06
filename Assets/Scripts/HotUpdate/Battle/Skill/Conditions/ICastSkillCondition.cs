using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Skill.Conditions
{
    /// <summary>
    /// 释放技能条件接口
    /// </summary>
    public interface ICastSkillCondition
    {
        /// <summary>
        /// 能否释放    
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        bool CanCast(IBattleEntityObject caster, ISkill skill);
    }
}
