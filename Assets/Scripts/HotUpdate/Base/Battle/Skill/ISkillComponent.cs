using System.Collections.Generic;

namespace HotUpdate.Base.Battle.Skill
{
    public interface ISkillComponent : IBattleComponent
    {
        /// <summary>
        /// 获取所有技能
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISkill> GetSkills();
    }
}
