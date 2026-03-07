using System.Collections;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Skill.Handler
{
    /// <summary>
    /// 基础技能释放后处理器
    /// </summary>
    public class BaseSkillCastPostHandler : ISkillCastPostHandler
    {
        public IEnumerator Handle(ISkill skill)
        {
            skill.Caster.CanAct = false;
            yield break;
        }
    }
}
