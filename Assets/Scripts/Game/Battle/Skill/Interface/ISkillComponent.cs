using System.Collections.Generic;
using Game.Battle.Component;

namespace Game.Battle.Skill.Interface
{
    /// <summary>
    /// ��������ӿ�
    /// </summary>
    public interface ISkillComponent : IBattleComponent
    {
        /// <summary>
        /// �Ƿ��ͷ�
        /// </summary>
        bool IsRelease { get; }

        /// <summary>
        /// ��ȡ���еļ���
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISkill> GetSkills();
    }
}
