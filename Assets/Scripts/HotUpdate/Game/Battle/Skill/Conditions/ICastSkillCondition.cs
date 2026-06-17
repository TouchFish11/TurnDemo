using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Object;

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
        /// <param name="skillInfo"></param>
        /// <returns></returns>
        bool CanCast(IBattleEntityObject caster, SkillInfo skillInfo);
    }
}
