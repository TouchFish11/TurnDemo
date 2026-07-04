using System.Collections;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 深渊法师技能释放后处理器
    /// </summary>
    public class AbyssalMageSkillCastPostHandler : SkillCastPostHandler
    {
        protected override IEnumerator OnHandle()
        {
            if (SkillContext.SkillInfo.f_id is not (105 or 106))
                yield break;
            
            var skillId = ((MonsterObject)SkillContext.Caster).SelectSkill();
            SkillContext.Caster.CastSkill(skillId);
        }
    }
}
