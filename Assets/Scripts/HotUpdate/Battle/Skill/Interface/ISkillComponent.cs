using System.Collections.Generic;
using HotUpdate.Battle.Core;
using HotUpdate.Battle.Skill.Base;

namespace HotUpdate.Battle.Skill.Interface
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
