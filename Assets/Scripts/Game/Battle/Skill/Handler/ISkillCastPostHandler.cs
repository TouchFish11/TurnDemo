using System.Collections;

namespace Game.Battle.Skill.Handler
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
        IEnumerator OnHandle(ISkill skill);
    }
}
