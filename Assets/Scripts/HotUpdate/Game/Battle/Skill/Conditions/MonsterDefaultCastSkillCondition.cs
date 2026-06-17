using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.Skill.Conditions
{
    /// <summary>
    /// 怪物默认释放技能条件
    /// </summary>
    public class MonsterDefaultCastSkillCondition : ICastSkillCondition
    {
        public bool CanCast(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            return skillInfo.f_SkillType.ToSkillType() switch
            {
                E_SkillType.Monster => true,
                _ => false
            };
        }
    }
}
