using System.Collections;

namespace HotUpdate.Game.Battle.Skill
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
        IEnumerator Handle(ISkill skill);
    }
}
