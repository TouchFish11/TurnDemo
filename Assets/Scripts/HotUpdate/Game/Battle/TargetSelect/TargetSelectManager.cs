using System;
using System.Collections.Generic;
using Core.Log;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Utility;

namespace HotUpdate.Game.Battle.TargetSelect
{
    /// <summary>
    /// 目标选择管理器
    /// 核心职责：管理战斗中技能释放的目标选择逻辑，维护选中的主目标和范围目标列表，
    /// 响应技能选择、拖拽切换目标、点击选中目标等交互事件，同步更新目标选择UI
    /// 单例模式实现，全局唯一管理战斗目标选择流程
    /// </summary>
    public class TargetSelectManager : ITargetSelectManager
    {
        // 缓存筛选出的所有目标
        private List<IBattleEntityObject> _filterEntitys;
        // 当前选中的主目标（技能优先作用的核心目标）
        private IBattleEntityObject _mainTarget;
        // 已选中的范围目标列表（包含主目标及范围内的其他目标）
        private readonly List<IBattleEntityObject> _selectedTargets = new();
        // 当前生效的目标选择策略（不同技能有不同的目标选择规则）
        private ITargetSelectStrategy _currentSelectStrategy;

        public void Init(IBattleContext context)
        {
            
        }
        
        /// <summary>
        /// 点击选中主目标
        /// 点击战斗实体时触发，直接将该实体设为主目标
        /// </summary>
        /// <param name="mainTarget">点击选中的战斗实体</param>
        public void SelectMainTarget(IBattleEntityObject mainTarget)
        {
            _mainTarget = mainTarget;
        }
        
        /// <summary>
        /// 根据技能、释放者、选择策略重新计算并更新主目标和范围目标
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <param name="caster">技能释放者</param>
        /// <param name="skillInfo">当前选中的技能配置</param>
        /// <param name="targetSelectStrategy">目标选择策略（决定如何选主目标）</param>
        public void SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo, ITargetSelectStrategy targetSelectStrategy)
        {
            _currentSelectStrategy = targetSelectStrategy ?? throw new ArgumentNullException(nameof(targetSelectStrategy));
            // 筛选出符合技能条件的所有目标
            FilterTargets(context, skillInfo, caster);
            // 委托给当前策略计算主目标
            SelectMainTarget(_currentSelectStrategy.SelectMainTarget(_filterEntitys, caster, skillInfo));
            if (_mainTarget == null)
            {
                Logger.LogError(ELogTags.Battle, $"{nameof(TargetSelectManager)}：当前选择的主目标为null");
                return;
            }
            
            Logger.LogDebug(ELogTags.Battle, $"当前主目标：{_mainTarget}");
        }
        
        /// <summary>
        /// 更新设置所有目标列表
        /// 基于主目标和技能范围规则，重新计算所有受影响的目标
        /// </summary>
        public void SelectAllTargets(int skillRangeType)
        {
            // 清空旧的范围目标列表
            _selectedTargets.Clear();
            // 计算主目标范围内的所有有效目标（玩家角色类型，按技能范围规则筛选）
            BattleUtility.GetRangeTargets(_mainTarget, skillRangeType, _filterEntitys, _selectedTargets);
        }
        
        /// <summary>
        /// 获取当前选中的主目标
        /// </summary>
        /// <returns>主目标战斗实体</returns>
        public IBattleEntityObject GetMainTarget()
        {
            return _mainTarget;
        }

        /// <summary>
        /// 获取当前选中的所有范围目标（包含主目标）
        /// </summary>
        /// <returns>范围目标列表</returns>
        public List<IBattleEntityObject> GetTargets()
        {
            return _selectedTargets;
        }

        /// <summary>
        /// 筛选目标
        /// </summary>
        /// <param name="context"></param>
        /// <param name="skillInfo"></param>
        /// <param name="caster"></param>
        private void FilterTargets(IBattleContext context, SkillInfo skillInfo, IBattleEntityObject caster)
        {
            // 从技能配置中解析目标类型（敌人/友方）
            var targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;
            // 根据施法者类型（玩家/怪物）筛选对应目标
            switch (caster)
            {
                // 施法者为玩家的情况
                case PlayerObject:
                {
                    _filterEntitys = targetType switch
                    {
                        E_SkillTargetType.Enemy => context.SceneMonsterObjects,
                        E_SkillTargetType.Friend => context.SceneRoleObjects,
                        _ => _filterEntitys
                    };
                    break;
                }
                // 施法者为怪物的情况
                case MonsterObject:
                {
                    _filterEntitys = targetType switch
                    {
                        E_SkillTargetType.Enemy => context.SceneRoleObjects,
                        E_SkillTargetType.Friend => context.SceneMonsterObjects,
                        _ => _filterEntitys
                    };
                    break;
                }
                default:
                    Logger.LogDebug(ELogTags.Battle, $"施法者不是：PlayerObject或MonsterObject");
                    break;
            }
        }
        
        /// <summary>
        /// 切换到下一个主目标
        /// 右拖拽交互触发，在同类型目标列表中向后切换主目标
        /// </summary>
        public void SelectNextMainTarget()
        {
            // 仅1个目标时无需切换
            if (_filterEntitys.Count <= 1)
            {
                return;
            }

            // 获取当前主目标在列表中的索引
            var mainIndex = _filterEntitys.IndexOf(_mainTarget);
            
            // 找不到目标，重置到中间
            if (mainIndex == -1)
            {
                mainIndex = _filterEntitys.Count / 2;
                _mainTarget = _filterEntitys[mainIndex];
                Logger.LogError(ELogTags.Battle, $"{nameof(TargetSelectManager)}.{nameof(SelectNextMainTarget)}：找不到目标，重置到中间");
            }
            
            // 索引未越界时，切换到下一个目标
            if (mainIndex + 1 < _filterEntitys.Count)
            {
                _mainTarget = _filterEntitys[++mainIndex];
                Logger.LogDebug(ELogTags.Battle, $"当前主目标：{_mainTarget}");
            }
        }

        /// <summary>
        /// 切换到上一个主目标
        /// 左拖拽交互触发，在同类型目标列表中向前切换主目标
        /// </summary>
        public void SelectPreviousMainTarget()
        {
            // 仅1个目标时无需切换
            if (_filterEntitys.Count <= 1)
            {
                return;
            }
            
            // 获取当前主目标在列表中的索引
            var mainIndex = _filterEntitys.IndexOf(_mainTarget);
            
            // 找不到目标，重置到中间
            if (mainIndex == -1)
            {
                mainIndex = _filterEntitys.Count / 2;
                _mainTarget = _filterEntitys[mainIndex];
                Logger.LogError(ELogTags.Battle, $"{nameof(TargetSelectManager)}.{nameof(SelectNextMainTarget)}：找不到目标，重置到中间");
            }
            
            // 索引未越界时，切换到上一个目标
            if (mainIndex - 1 >= 0)
            {
                _mainTarget = _filterEntitys[--mainIndex];
                Logger.LogDebug(ELogTags.Battle, $"当前主目标：{_mainTarget}");
            }
        }

        public void Reset()
        {
            _filterEntitys.Clear();
            _selectedTargets.Clear();
            _mainTarget = null;
            _currentSelectStrategy = null;
        }
    }
}