using System.Collections.Generic;
using GameHotUpdate.Battle.Core;
using GameHotUpdate.Battle.Skill.Base;

namespace GameHotUpdate.Battle.Skill.Interface
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
