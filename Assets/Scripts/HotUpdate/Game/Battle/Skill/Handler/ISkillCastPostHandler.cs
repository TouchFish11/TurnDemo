using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Handler
{
    /// <summary>
    /// 技能释放后处理器
    /// </summary>
    public interface ISkillCastPostHandler
    {
        /// <summary>
        /// 处理逻辑
        /// </summary>
        /// <returns></returns>
        IEnumerator Handle(SkillContext skillContext);
    }
}
