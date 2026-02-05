using System.Collections;
using Game.Battle.Skill;
using Game.Battle.Skill.Handler;

namespace GameHotUpdate.Battle.Skill.Handlers
{
    /// <summary>
    /// ���������ͷź�����
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
