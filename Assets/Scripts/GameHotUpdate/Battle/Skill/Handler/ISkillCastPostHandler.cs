using System.Collections;
using GameHotUpdate.Battle.Skill.Base;

namespace GameHotUpdate.Battle.Skill.Handler
{
    /// <summary>
    /// �����ͷź�����
    /// </summary>
    public interface ISkillCastPostHandler
    {
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        IEnumerator Handle(ISkill skill);
    }
}
