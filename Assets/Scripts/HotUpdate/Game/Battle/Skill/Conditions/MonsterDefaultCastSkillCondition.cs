using HotUpdate.Base;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.Skill.Conditions
{
    /// <summary>
    /// 怪物默认释放技能条件
    /// </summary>
    public class MonsterDefaultCastSkillCondition : ICastSkillCondition
    {
        public bool CanCast(IBattleEntityObject caster, ISkill skill)
        {
            switch (skill.SkillInfo.f_SkillType.ToSkillType())
            {
                case E_SkillType.Monster:
                    return true;
                default:
                    return false;
            }
        }
    }
}
