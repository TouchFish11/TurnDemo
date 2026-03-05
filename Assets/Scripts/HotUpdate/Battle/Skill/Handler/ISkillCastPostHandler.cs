using System.Collections;
using HotUpdate.Battle.Skill.Base;

namespace HotUpdate.Battle.Skill.Handler
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
