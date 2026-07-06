using System;
using System.Collections.Generic;
using Core.DI;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.Component;

using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能组件核心逻辑
    /// </summary>
    public class SkillComponentCore : ComponentCore<SkillComponent>
    {
        [Inject] protected BinaryDataManager binaryDataManager;
        
        // 技能工厂
        protected ISkillFactory skillFactory;
        // 技能数据字典：Key为技能ID，Value为对应的技能数据对象，用于快速索引技能
        protected List<int> skillIds = new();
        // 施法条件集合：存储当前技能组件生效的所有施法条件，施法前需校验所有条件
        protected List<ICastSkillCondition> castSkillConditions = new();
        // 目标选择策略集合：存储当前技能组件的所有目标选择策略，按优先级排序后生效
        protected List<ITargetSelectStrategy> targetSelectStrategies = new();
        
        public int SkillCount => skillIds.Count;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="f_skillIds"></param>
        /// <param name="skillFactory"></param>
        public void InitSkill(string f_skillIds, ISkillFactory skillFactory)
        {
            // 将技能ID字符串解析为int数组（第二个参数2为分隔符标识，需参考TextUtility.SplitToIntArr实现）
            var skillIds = TextUtility.SplitToIntArr(f_skillIds, 2);
            foreach (var skillId in skillIds)
            {
                this.skillIds.Add(skillId);
            }
            this.skillFactory = skillFactory;
        }
        
        /// <summary>
        /// 新增技能到当前组件
        /// 注：仅当技能ID未存在时才会添加，避免重复
        /// </summary>
        /// <param name="skillId">要添加的技能ID</param>
        public void AddSkill(int skillId)
        {
            skillIds.Add(skillId);
        }
        
        /// <summary>
        /// 添加施法条件到当前组件
        /// 注：仅当条件未存在时才会添加，避免重复
        /// </summary>
        /// <param name="castSkillCondition">要添加的施法条件实例</param>
        public void AddCastCondition(ICastSkillCondition castSkillCondition)
        {
            if (!castSkillConditions.Contains(castSkillCondition))
            {
                castSkillConditions.Add(castSkillCondition);
            }
        }

        /// <summary>
        /// 从当前组件移除指定施法条件
        /// </summary>
        /// <param name="castSkillCondition">要移除的施法条件实例</param>
        public void RemoveCastCondition(ICastSkillCondition castSkillCondition)
        {
            castSkillConditions.Remove(castSkillCondition);
        }

        /// <summary>
        /// 添加目标选择策略到当前组件
        /// 添加后会自动重新排序策略（按优先级降序）
        /// </summary>
        /// <param name="targetSelectStrategy">要添加的目标选择策略实例</param>
        public void AddTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            targetSelectStrategies.Add(targetSelectStrategy);
            SortTargetStratgy(); // 添加后排序，保证策略优先级生效
        }

        /// <summary>
        /// 从当前组件移除指定目标选择策略
        /// 移除后会自动重新排序策略（按优先级降序）
        /// </summary>
        /// <param name="targetSelectStrategy">要移除的目标选择策略实例</param>
        public void RemoveTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            targetSelectStrategies.Remove(targetSelectStrategy);
            SortTargetStratgy(); // 移除后排序，保证策略优先级生效
        }

        /// <summary>
        /// 目标选择策略排序
        /// 按策略优先级降序排列（优先级高的排在前面，优先生效）
        /// </summary>
        private void SortTargetStratgy()
        {
            targetSelectStrategies.Sort((s1, s2) =>
            {
                if (s1.Priority > s2.Priority)
                {
                    return -1; // s1优先级更高，排在前面
                }
                else if(s1.Priority < s2.Priority)
                {
                    return 1; // s2优先级更高，排在前面
                }
                else
                {
                    return 0;
                }
            });
        }
        
        /// <summary>
        /// 能否释放技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public bool CanCast(int skillId)
        {
            // 先校验技能是否存在
            if (skillIds.Contains(skillId))
            {
                var skillInfo = binaryDataManager.GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
                // 遍历所有施法条件，只要有一个条件不满足则返回false
                foreach (var condition in castSkillConditions)
                {
                    if (!condition.CanCast(Component.BattleEntity, skillInfo))
                    {
                        return false;
                    }
                }
            }
            else
            {
                // 技能不存在，返回false
                return false;
            }
            
            // 所有校验通过，返回true
            return true;
        }

        /// <summary>
        /// 获取当前组件管理的所有技能ID列表
        /// </summary>
        /// <returns>技能ID的新列表（避免外部修改原字典）</returns>
        public IEnumerable<int> GetSkillIds()
        {
            foreach (var skillId in skillIds)
            {
                yield return skillId;
            }
        }

        /// <summary>
        /// 获取指定ID的技能数据
        /// 获取技能数据后，自动为技能设置目标选择策略
        /// </summary>
        /// <param name="skillId">要获取的技能ID</param>
        /// <returns>对应的技能数据对象</returns>
        public ISkill GetSkill(int skillId)
        {
            // 为技能设置最高优先级的目标选择策略（排序后第一个即为最高优先级）
            return skillFactory.CreateSkill(Component.BattleEntity, skillId, targetSelectStrategies[0]);
        }

        public int GetSkillIdByIndex(int index)
        {
            if(index < 0 || index >= skillIds.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            return skillIds[index];
        }

        protected override void OnDispose()
        {
            binaryDataManager = null;
            skillFactory = null;
            skillIds.Clear();
            skillIds = null;
            castSkillConditions.Clear();
            castSkillConditions = null;
            targetSelectStrategies.Clear();
            targetSelectStrategies = null;
        }
    }
}
