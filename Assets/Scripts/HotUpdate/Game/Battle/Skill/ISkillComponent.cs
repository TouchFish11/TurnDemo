using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Skill
{
    public interface ISkillComponent
    {
        /// <summary>
        /// 获取所有技能
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISkill> GetSkills();
    }
}
