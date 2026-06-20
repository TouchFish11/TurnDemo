using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Handler
{
    /// <summary>
    /// 基础技能释放后处理器
    /// </summary>
    public class BaseSkillCastPostHandler : ISkillCastPostHandler
    {
        public IEnumerator Handle(SkillContext skillContext)
        {
            skillContext.Caster.CanAct = false;
            yield break;
        }
    }
}
