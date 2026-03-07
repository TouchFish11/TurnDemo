using HotUpdate.Battle.Utility;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Skill.Conditions
{
    /// <summary>
    /// ����Ĭ���ͷż�������
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
