using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Enum;
using HotUpdate.Extension;

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
