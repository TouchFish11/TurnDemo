using System.Collections;

namespace HotUpdate.Core.Battle.Skill
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
