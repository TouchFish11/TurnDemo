using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Battle.Skill.Enum;
using GameHotUpdate.Extension;

namespace GameHotUpdate.Battle.Skill.Conditions
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
