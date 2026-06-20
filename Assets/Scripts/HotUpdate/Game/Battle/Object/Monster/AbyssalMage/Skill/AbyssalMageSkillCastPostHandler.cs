using System.Collections;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 
    /// </summary>
    public class AbyssalMageSkillCastPostHandler : ISkillCastPostHandler
    {
        public IEnumerator Handle(SkillContext skillContext)
        {
            if (skillContext.SkillInfo.f_id is not (105 or 106))
            {
                skillContext.Caster.CanAct = false;
                yield break;
            }
            
            skillContext.Caster.CanAct = true;
            var skillId = ((MonsterObject)skillContext.Caster).SelectSkill();
            skillContext.Caster.CastSkill(skillId);
        }
    }
}
