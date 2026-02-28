using System;
using System.Collections.Generic;
using Core.Log;
using Core.Serialize.Binary;
using Core.Service;
using Core.Singleton;
using Game.Battle.Context;
using Game.Battle.Input;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using Game.Battle.TargetSelect;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Utility;

namespace GameHotUpdate.Battle.TargetSelect
{
    /// <summary>
    /// 目标选择管理器
    /// 核心职责：管理战斗中技能释放的目标选择逻辑，维护选中的主目标和范围目标列表，
    /// 响应技能选择、拖拽切换目标、点击选中目标等交互事件，同步更新目标选择UI
    /// 单例模式实现，全局唯一管理战斗目标选择流程
    /// </summary>
    public class TargetSelectManager : SingletonBase<TargetSelectManager>, ITargetSelectManager
    {
        // 缓存筛选出的所有目标
        private List<IBattleEntityObject> _filterEntitys;
        // 已选中的范围目标列表（包含主目标及范围内的其他目标）
        private readonly List<IBattleEntityObject> _selectedTargets = new();
        // 当前选中的主目标（技能优先作用的核心目标）
        private IBattleEntityObject _mainTarget;
        // 当前选中技能的配置信息
        private SkillInfo skillInfo;
        // 战斗上下文
        private IBattleContext battleContext;
        // 技能释放者（释放当前技能的战斗实体）
        private IBattleEntityObject caster;
        // 当前生效的目标选择策略（不同技能有不同的目标选择规则）
        private ITargetSelectStrategy currentSelectStrategy;
        
        /// <summary>
        /// 主目标选择变化
        /// </summary>
        public event Action<IBattleEntityObject> OnSelectChanged;
        
        /// <summary>
        /// 私有构造函数
        /// 单例模式：禁止外部实例化，通过 SingletonBase 的 Instance 属性获取实例
        /// </summary>
        private TargetSelectManager()
        {

        }

        /// <summary>
        /// 初始化目标选择管理器
        /// </summary>
        /// <param name="battleContext">战斗上下文，提供战斗核心数据和事件总线</param>
        public void Init(IBattleContext battleContext)
        {
            this.battleContext = battleContext;
            // 注册技能选择事件监听：当玩家选择技能时触发目标选择逻辑
            battleContext.GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
        }
        
        /// <summary>
        /// 技能选择事件回调
        /// 当玩家在UI中选中某个技能时触发，初始化目标选择的核心数据
        /// </summary>
        /// <param name="selectSkillEvent">技能选择事件（携带技能ID、释放者、战斗上下文等）</param>
        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            // 从配置表加载选中技能的详细配置
            var skillInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[selectSkillEvent.SkillId];
            // 触发目标选择逻辑
            SelectTarget(selectSkillEvent.Context, selectSkillEvent.Caster, skillInfo, selectSkillEvent.TargetSelectStrategy);
        }

        /// <summary>
        /// 激活目标选择交互
        /// 开启拖拽/点击切换目标的交互能力
        /// </summary>
        public void ActiveSelectTarget()
        {
            InActiveSelectTarget();
            
            ServiceLocator.Get<IBattleInputHandler>().OnLeftDrag += SelectPreviousMainTarget;   // 左拖拽：切换上一个主目标
            ServiceLocator.Get<IBattleInputHandler>().OnRightDrag += SelectNextMainTarget;     // 右拖拽：切换下一个主目标
            ServiceLocator.Get<IBattleInputHandler>().OnSelectedObject += SelectClickMainTarget;// 点击：选中指定主目标
            
            LogManager.Log($"激活目标选择");
        }

        /// <summary>
        /// 禁用目标选择交互
        /// 关闭拖拽/点击切换目标的交互能力，避免无效输入响应
        /// </summary>
        public void InActiveSelectTarget()
        {
            ServiceLocator.Get<IBattleInputHandler>().OnLeftDrag -= SelectPreviousMainTarget;
            ServiceLocator.Get<IBattleInputHandler>().OnRightDrag -= SelectNextMainTarget;
            ServiceLocator.Get<IBattleInputHandler>().OnSelectedObject -= SelectClickMainTarget;
            
            LogManager.Log($"禁用目标选择");
        }

        /// <summary>
        /// 核心目标选择入口
        /// 根据技能、释放者、选择策略重新计算并更新主目标和范围目标
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <param name="caster">技能释放者</param>
        /// <param name="skillInfo">当前选中的技能配置</param>
        /// <param name="targetSelectStrategy">目标选择策略（决定如何选主目标）</param>
        public void SelectTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo, ITargetSelectStrategy targetSelectStrategy)
        {
            // 缓存当前内容 
            this.skillInfo = skillInfo;
            this.caster = caster;
            currentSelectStrategy = targetSelectStrategy;
            
            if (currentSelectStrategy == null)
            {
                LogManager.LogError($"{nameof(TargetSelectManager)}.{nameof(SelectTarget)}：当前目标选择策略为null");
                return;
            }
            
            // 技能切换时，先重新选择主目标
            _mainTarget = SelectMainTarget(context, caster, skillInfo);
            if (_mainTarget == null)
            {
                LogManager.LogError($"{nameof(TargetSelectManager)}.{nameof(SelectMainTarget)}：当前选择的主目标为null");
                return;
            }
            
            OnSelectChanged?.Invoke(_mainTarget);
            LogManager.Log($"当前主目标：{_mainTarget}");
            // 基于主目标更新范围目标列表
            UpdateTargets();
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
        /// 选择主目标
        /// 基于当前选择策略，计算并设置技能的核心作用目标
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <param name="caster">技能释放者</param>
        /// <param name="skillInfo">技能配置（影响目标选择规则）</param>
        private IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // 筛选出所有目标
            FilterTargets(context);
            // 委托给当前策略计算主目标
            return currentSelectStrategy.SelectMainTarget(_filterEntitys, caster, skillInfo);
        }

        /// <summary>
        /// 筛选目标
        /// </summary>
        /// <param name="context"></param>
        private void FilterTargets(IBattleContext context)
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
                        E_SkillTargetType.Enemy => context.GetSceneMonsters(),
                        E_SkillTargetType.Friend => context.GetSceneRoles(),
                        _ => _filterEntitys
                    };
                    break;
                }
                // 施法者为怪物的情况
                case MonsterObject:
                {
                    _filterEntitys = targetType switch
                    {
                        E_SkillTargetType.Enemy => context.GetSceneRoles(),
                        E_SkillTargetType.Friend => context.GetSceneMonsters(),
                        _ => _filterEntitys
                    };
                    break;
                }
                default:
                    LogManager.Log($"施法者不是：PlayerObject或MonsterObject");
                    break;
            }
        }

        /// <summary>
        /// 更新范围目标列表
        /// 基于主目标和技能范围规则，重新计算所有受影响的目标，并触发UI更新事件
        /// </summary>
        private void UpdateTargets()
        {
            // 清空旧的范围目标列表
            _selectedTargets.Clear();
            // 计算主目标范围内的所有有效目标（玩家角色类型，按技能范围规则筛选）
            BattleUtil.GetRangeTargets(_mainTarget, skillInfo.f_skillRangeType, _filterEntitys, _selectedTargets);
            // 触发目标选择变更事件，通知UI更新选中状态
            battleContext.GetEventBus().TriggerEvent(new SelectTargetEvent(battleContext, caster, _mainTarget, _selectedTargets));
        }

        /// <summary>
        /// 切换到下一个主目标
        /// 右拖拽交互触发，在同类型目标列表中向后切换主目标
        /// </summary>
        private void SelectNextMainTarget()
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
                LogManager.LogError($"{nameof(TargetSelectManager)}.{nameof(SelectNextMainTarget)}：找不到目标，重置到中间");
            }
            
            // 索引未越界时，切换到下一个目标
            if (mainIndex + 1 < _filterEntitys.Count)
            {
                _mainTarget = _filterEntitys[++mainIndex];
                OnSelectChanged?.Invoke(_mainTarget);
                LogManager.Log($"当前主目标：{_mainTarget}");
                // 切换后更新范围目标列表并同步UI
                UpdateTargets();
            }
        }

        /// <summary>
        /// 切换到上一个主目标
        /// 左拖拽交互触发，在同类型目标列表中向前切换主目标
        /// </summary>
        private void SelectPreviousMainTarget()
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
                LogManager.LogError($"{nameof(TargetSelectManager)}.{nameof(SelectNextMainTarget)}：找不到目标，重置到中间");
            }
            
            // 索引未越界时，切换到上一个目标
            if (mainIndex - 1 >= 0)
            {
                _mainTarget = _filterEntitys[--mainIndex];
                OnSelectChanged?.Invoke(_mainTarget);
                LogManager.Log($"当前主目标：{_mainTarget}");
                // 切换后更新范围目标列表并同步UI
                UpdateTargets();
            }
        }

        /// <summary>
        /// 点击选中主目标
        /// 点击战斗实体时触发，直接将该实体设为主目标
        /// </summary>
        /// <param name="mainTarget">点击选中的战斗实体</param>
        private void SelectClickMainTarget(IBattleEntityObject mainTarget)
        {
            _mainTarget = mainTarget;
            // 选中后更新范围目标列表并同步UI
            UpdateTargets();
        }
    }
}