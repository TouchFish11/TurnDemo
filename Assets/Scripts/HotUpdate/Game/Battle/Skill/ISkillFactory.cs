using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill
{
    /// <summary>
    /// 技能工厂接口
    /// </summary>
    public interface ISkillFactory
    {
        /// <summary>
        /// 创建技能实例
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="skillId">技能ID</param>
        /// <param name="targetSelectStrategy"></param>
        /// <returns>技能数据</returns>
        ISkill CreateSkill(IBattleEntityObject caster, int skillId, ITargetSelectStrategy targetSelectStrategy);
    }
}