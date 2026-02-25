using System.Collections;
using Game.Battle.Skill;
using Game.Battle.Skill.Handler;

namespace GameHotUpdate.Battle.Skill.Handlers
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
