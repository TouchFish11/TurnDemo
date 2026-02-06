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
        /// 获取所有技能
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISkill> GetSkills();
    }
}
