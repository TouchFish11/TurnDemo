using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Skill.Conditions
{
    /// <summary>
    /// 怪物默认释放技能条件
    /// </summary>
    public class MonsterDefaultCastSkillCondition : ICastSkillCondition
    {
        public bool CanCast(IBattleEntityObject caster, SkillInfo skillInfo)
        {
            return (E_SkillType)skillInfo.f_SkillType switch
            {
                E_SkillType.Monster => true,
                _ => false
            };
        }
    }
}
