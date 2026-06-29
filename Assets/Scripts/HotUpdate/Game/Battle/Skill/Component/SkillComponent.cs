using System.Collections.Generic;
using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill.Component
{
    /// <summary>
    /// 技能组件抽象基类
    /// 负责管理战斗实体的技能数据、施法条件、目标选择策略，提供技能相关的核心操作能力
    /// 所有具体的技能组件（如角色技能组件、怪物技能组件）需继承此类实现具体逻辑
    /// </summary>
    [ComponentCore(typeof(SkillComponentCore))]
    public abstract class SkillComponent : BattleComponent, ISkillComponent
    {
        protected SkillComponentCore skillComponentCore;
        
        public int SkillCount => skillComponentCore.SkillCount;

        protected override void OnBattleInit()
        {
            skillComponentCore = (SkillComponentCore)ComponentCore;
        }

        /// <summary>
        /// 校验指定技能是否可以释放
        /// </summary>
        /// <param name="skillId">要校验的技能ID</param>
        /// <returns>true=可释放；false=不可释放（技能不存在/任意施法条件不满足）</returns>
        public bool CanCast(int skillId)
        {
            return skillComponentCore.CanCast(skillId);
        }

        /// <summary>
        /// 新增技能到当前组件
        /// 注：仅当技能ID未存在时才会添加，避免重复
        /// </summary>
        /// <param name="skillId">要添加的技能ID</param>
        public void AddSkill(int skillId)
        {
            skillComponentCore.AddSkill(skillId);
        }

        /// <summary>
        /// 添加施法条件到当前组件
        /// 注：仅当条件未存在时才会添加，避免重复
        /// </summary>
        /// <param name="castSkillCondition">要添加的施法条件实例</param>
        public void AddCastCondition(ICastSkillCondition castSkillCondition)
        {
            skillComponentCore.AddCastCondition(castSkillCondition);
        }

        /// <summary>
        /// 从当前组件移除指定施法条件
        /// </summary>
        /// <param name="castSkillCondition">要移除的施法条件实例</param>
        public void RemoveCastCondition(ICastSkillCondition castSkillCondition)
        {
            skillComponentCore.RemoveCastCondition(castSkillCondition);
        }

        /// <summary>
        /// 添加目标选择策略到当前组件
        /// 添加后会自动重新排序策略（按优先级降序）
        /// </summary>
        /// <param name="targetSelectStrategy">要添加的目标选择策略实例</param>
        public void AddTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            skillComponentCore.AddTargetSelectStrategy(targetSelectStrategy);
        }

        /// <summary>
        /// 从当前组件移除指定目标选择策略
        /// 移除后会自动重新排序策略（按优先级降序）
        /// </summary>
        /// <param name="targetSelectStrategy">要移除的目标选择策略实例</param>
        public void RemoveTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            skillComponentCore.RemoveTargetSelectStrategy(targetSelectStrategy);
        }

        /// <summary>
        /// 获取当前组件管理的所有技能ID列表
        /// </summary>
        /// <returns>技能ID的新列表（避免外部修改原字典）</returns>
        public IEnumerable<int> GetSkillIds()
        {
            return skillComponentCore.GetSkillIds();
        }

        /// <summary>
        /// 获取指定ID的技能数据
        /// 获取技能数据后，自动为技能设置目标选择策略
        /// </summary>
        /// <param name="skillId">要获取的技能ID</param>
        /// <returns>对应的技能数据对象</returns>
        public ISkill GetSkill(int skillId)
        {
            return skillComponentCore.GetSkill(skillId);
        }

        public int GetSkillIdByIndex(int index)
        {
            return skillComponentCore.GetSkillIdByIndex(index);
        }

        protected override void OnBattleDestroy()
        {
            skillComponentCore = null;
        }
    }
}