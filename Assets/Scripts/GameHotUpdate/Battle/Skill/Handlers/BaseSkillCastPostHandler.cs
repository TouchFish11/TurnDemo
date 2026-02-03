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
            // �����ж�����
            skill.Caster.SubActCount();
            yield break;
        }
    }
}
