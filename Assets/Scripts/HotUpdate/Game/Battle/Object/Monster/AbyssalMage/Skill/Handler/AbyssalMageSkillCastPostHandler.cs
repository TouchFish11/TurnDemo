using System.Collections;
using HotUpdate.Game.Battle.Skill;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Handler
{
    /// <summary>
    /// 
    /// </summary>
    public class AbyssalMageSkillCastPostHandler : ISkillCastPostHandler
    {
        public IEnumerator Handle(ISkill skill)
        {
            if (skill.SkillInfo.f_id is not (105 or 106))
            {
                skill.Caster.CanAct = false;
                yield break;
            }
            
            skill.Caster.CanAct = true;
            var skillId = ((MonsterObject)skill.Caster).SelectSkill();
            skill.Caster.CastSkill(skillId);
        }
    }
}
