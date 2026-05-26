using System.Collections.Generic;
using HotUpdate.Base;

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
        /// <param name="caster">施法者</param>
        /// <param name="skillId">技能ID</param>
        /// <returns>技能数据</returns>
        ISkillData CreateSkill(IBattleEntityObject caster, int skillId);
        
        /// <summary>
        /// 批量创建技能实例
        /// </summary>
        /// <param name="caster">施法者</param>
        /// <param name="skillIds">技能ID数组</param>
        /// <returns>技能数据集合</returns>
        IEnumerable<ISkillData> CreateSkills(IBattleEntityObject caster, params int[] skillIds);
    }
}