using HotUpdate.Base;

namespace HotUpdate.Game.Battle.Skill.Conditions
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
