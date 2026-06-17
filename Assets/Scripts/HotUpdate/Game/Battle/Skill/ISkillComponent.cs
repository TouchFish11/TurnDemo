using System.Collections.Generic;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill
{
    public interface ISkillComponent
    {
        /// <summary>
        /// 添加目标选择策略到当前组件
        /// 添加后会自动重新排序策略（按优先级降序）
        /// </summary>
        /// <param name="targetSelectStrategy">要添加的目标选择策略实例</param>
        void AddTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy);

        /// <summary>
        /// 校验指定技能是否可以释放
        /// </summary>
        /// <param name="skillId">要校验的技能ID</param>
        /// <returns>true=可释放；false=不可释放（技能不存在/任意施法条件不满足）</returns>
        bool CanCast(int skillId);

        /// <summary>
        /// 新增技能到当前组件
        /// 注：仅当技能ID未存在时才会添加，避免重复
        /// </summary>
        /// <param name="skillId">要添加的技能ID</param>
        void AddSkill(int skillId);

        /// <summary>
        /// 添加施法条件到当前组件
        /// 注：仅当条件未存在时才会添加，避免重复
        /// </summary>
        /// <param name="castSkillCondition">要添加的施法条件实例</param>
        void AddCastCondition(ICastSkillCondition castSkillCondition);

        /// <summary>
        /// 从当前组件移除指定目标选择策略
        /// 移除后会自动重新排序策略（按优先级降序）
        /// </summary>
        /// <param name="targetSelectStrategy">要移除的目标选择策略实例</param>
        void RemoveTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy);

        /// <summary>
        /// 获取当前组件管理的所有技能ID列表
        /// </summary>
        /// <returns>技能ID的新列表（避免外部修改原字典）</returns>
        List<int> GetSkillIds();

        /// <summary>
        /// 获取指定ID的技能数据
        /// 获取技能数据后，自动为技能设置目标选择策略
        /// </summary>
        /// <param name="skillId">要获取的技能ID</param>
        /// <returns>对应的技能数据对象</returns>
        ISkillData GetSkillData(int skillId);

        /// <summary>
        /// 从当前组件移除指定施法条件
        /// </summary>
        /// <param name="castSkillCondition">要移除的施法条件实例</param>
        void RemoveCastCondition(ICastSkillCondition castSkillCondition);
    }
}
