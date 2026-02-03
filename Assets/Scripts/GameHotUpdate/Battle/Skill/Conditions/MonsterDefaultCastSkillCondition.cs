using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Condition;
using Game.Battle.Skill.Enum;
using Game.Tasks;
using GameHotUpdate.Tasks;

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
