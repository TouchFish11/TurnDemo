using System.Collections.Generic;
using Core.Components;
using Core.Reflection;
using Core.Service;
using Core.Utility;
using Game.Battle.Component;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Toughness;
using Game.Tasks;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Toughness.CalcStrategy;
using GameHotUpdate.Battle.Toughness.ReduceStrategy;
using GameHotUpdate.Objects;
using UnityEngine;

namespace GameHotUpdate.Battle.Toughness
{
    /// <summary>
    /// 韧性组件
    /// 功能：管理战斗实体（怪物/角色）的韧性系统核心逻辑，包括韧性初始化、韧性值计算/扣除、破韧判断、策略管理等
    /// 依赖：BattleComponent（战斗组件基类）、IToughnessComponent（韧性组件接口）、策略模式（扣除/计算策略）、事件总线（状态变更通知）
    /// </summary>
    [ComponentId(typeof(ToughnessComponent))] // 标记组件唯一标识，用于组件注册和获取
    public class ToughnessComponent : BattleComponent, IToughnessComponent
    {
        // 当前韧性状态（封装了韧性值、最大值、弱点属性、破韧状态等核心数据）
        private ToughnessState _toughness;
        // 韧性扣除策略集合（不同规则的扣除判断逻辑，按优先级排序执行）
        private readonly List<IToughnessReduceStrategy> _toughnessReduceStrategies = new List<IToughnessReduceStrategy>();
        // 韧性计算策略集合（不同规则的韧性值计算逻辑，按优先级排序执行）
        private readonly List<IToughnessCalcStrategy> _toughnessCalcStrategies = new List<IToughnessCalcStrategy>();

        /// <summary>
        /// 初始化韧性组件（接口实现）
        /// </summary>
        /// <param name="owner">所属战斗实体（如怪物、角色）</param>
        /// <param name="elementTypes">弱点属性类型数组（整型枚举值）</param>
        /// <param name="initialToughness">初始韧性最大值</param>
        void IToughnessComponent.Init(IBattleEntityObject owner, int[] elementTypes, int initialToughness)
        {
            // 转换整型弱点属性为枚举类型
            var weakPropertys = new List<E_ElementType>(elementTypes.Length);
            foreach (var type in elementTypes)
            {
                weakPropertys.Add(type.ToElementType());
            }
            // 初始化韧性状态对象
            _toughness = new ToughnessState(weakPropertys, initialToughness);
        }

        /// <summary>
        /// 战斗初始化（重写基类方法）
        /// 时机：战斗实体初始化时调用，完成韧性组件的基础数据加载和默认策略注册
        /// </summary>
        /// <param name="battleEntity">所属战斗实体对象</param>
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);

            // 获取怪物信息组件（当前仅适配怪物，若适配角色需扩展）
            var monsterInfo = ((MonsterObject)battleEntity).MonsterInfo;
            // 解析怪物配置的弱点属性（字符串分割为整型数组）和基础韧性值，初始化韧性
            (this as IToughnessComponent).Init(battleEntity, TextUtility.SplitToIntArr(monsterInfo.f_weaknesses, 2), monsterInfo.f_baseToughness);

            // 注册默认韧性扣除策略（从策略工厂获取）
            var reduceStrategy = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<IToughnessStrategyFactory, ToughnessStrategyFactory>()
                .GetReduceStrategy<DefaultToughnessReduceStrategy>();
            _toughnessReduceStrategies.Add(reduceStrategy);
            
            // 注册默认韧性计算策略（从策略工厂获取）
            var calcStrategy = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<IToughnessStrategyFactory, ToughnessStrategyFactory>()
                .GetCalcStrategy<DefaultToughnessCalcStrategy>();
            _toughnessCalcStrategies.Add(calcStrategy);
        }

        /// <summary>
        /// 添加韧性扣除策略
        /// 说明：添加后自动按优先级重新排序，保证高优先级策略先执行
        /// </summary>
        /// <param name="reduceStrategy">待添加的扣除策略实例</param>
        public void AddToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy)
        {
            if (_toughnessReduceStrategies.Contains(reduceStrategy))
            {
                return;
            }
            
            _toughnessReduceStrategies.Add(reduceStrategy);
            // 按优先级排序策略
            SortReduceStrategy();
        }

        /// <summary>
        /// 移除韧性扣除策略
        /// 说明：移除后自动重新排序，保证策略执行顺序正确
        /// </summary>
        /// <param name="reduceStrategy">待移除的扣除策略实例</param>
        public void RemoveToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy)
        {
            if (_toughnessReduceStrategies.Remove(reduceStrategy))
            {
                // 按优先级重新排序
                SortReduceStrategy();
            }
        }

        /// <summary>
        /// 排序韧性扣除策略（私有）
        /// 规则：优先级数值越大，排序越靠前（先执行）；优先级相同则顺序不变
        /// </summary>
        private void SortReduceStrategy()
        {
            _toughnessReduceStrategies.Sort((s1, s2) =>
            {
                if (s1.Priority > s2.Priority)
                {
                    return -1; // 高优先级排前面
                }
                else if (s1.Priority < s2.Priority)
                {
                    return 1; // 低优先级排后面
                }
                else
                {
                    return 0; // 优先级相同，保持原有顺序
                }
            });
        }

        /// <summary>
        /// 添加韧性计算策略
        /// 说明：添加后自动按优先级重新排序，保证高优先级策略先执行
        /// </summary>
        /// <param name="calcStrategy">待添加的计算策略实例</param>
        public void AddToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy)
        {
            if (_toughnessCalcStrategies.Contains(calcStrategy))
            {
                return;
            }
            
            _toughnessCalcStrategies.Add(calcStrategy);
            // 修复：原代码此处错误调用SortReduceStrategy，修正为SortCalcStrategy
            SortCalcStrategy();
        }

        /// <summary>
        /// 移除韧性计算策略
        /// 说明：移除后自动重新排序，保证策略执行顺序正确
        /// </summary>
        /// <param name="calcStrategy">待移除的计算策略实例</param>
        public void RemoveToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy)
        {
            if (_toughnessCalcStrategies.Remove(calcStrategy))
            {
                // 按优先级重新排序
                SortCalcStrategy();
            }
        }

        /// <summary>
        /// 排序韧性计算策略（私有）
        /// 规则：优先级数值越大，排序越靠前（先执行）；优先级相同则顺序不变
        /// </summary>
        private void SortCalcStrategy()
        {
            _toughnessCalcStrategies.Sort((s1, s2) =>
            {
                if (s1.Priority > s2.Priority)
                {
                    return -1; // 高优先级排前面
                }
                else if (s1.Priority < s2.Priority)
                {
                    return 1; // 低优先级排后面
                }
                else
                {
                    return 0; // 优先级相同，保持原有顺序
                }
            });
        }

        /// <summary>
        /// 扣除韧性值（核心方法）
        /// 流程：1. 判断是否可扣除 → 2. 计算最终扣除值 → 3. 更新韧性值 → 4. 触发状态变更事件 → 5. 判断是否破韧并触发破韧事件
        /// </summary>
        /// <param name="reducer">扣除韧性的发起者（如攻击方角色/技能）</param>
        /// <param name="propertyType">触发扣除的属性类型</param>
        /// <param name="skillInfo">关联的技能信息（包含基础韧性扣除值等）</param>
        public void ReduceToughness(IBattleEntityObject reducer, E_ElementType propertyType, SkillInfo skillInfo)
        {
            // 前置判断：是否允许扣除韧性（已破韧/策略不允许则直接返回）
            if (!CanReduceToughness(reducer, propertyType, skillInfo.f_toughenValue))
            {
                return;
            }

            // 计算最终要扣除的韧性值（叠加所有计算策略的结果）
            var finalReduceValue = CalcToughness(reducer, propertyType, skillInfo.f_toughenValue);
            // 计算扣除后剩余韧性值（最小为0，避免负数）
            var current = Mathf.Max(0, _toughness.CurrentToughnessValue - finalReduceValue);
            // 更新韧性值（同步最大值，当前最大值暂未动态修改）
            SetToughnessValue(current, _toughness.MaxToughnessVaue);

            // 触发韧性值变更事件（供UI、其他组件监听）
            BattleEntity.Context.GetEventBus().TriggerEvent(
                new ToughnessChangedEvent(
                    BattleEntity.Context, 
                    BattleEntity, 
                    _toughness.CurrentToughnessValue, 
                    _toughness.MaxToughnessVaue
                )
            );
            
            // 判断是否触发破韧，若破韧则触发破韧事件（供眩晕、增伤等逻辑监听）
            if (IsToughnessBroken())
            {
                BattleEntity.Context.GetEventBus().TriggerEvent(new ToughnessBrokenEvent(BattleEntity.Context, reducer, BattleEntity, skillInfo));
            }
        }

        /// <summary>
        /// 设置韧性值（对外暴露的可控接口）
        /// 说明：修改韧性值后会主动触发状态变更事件，保证外部数据同步
        /// </summary>
        /// <param name="current">新的当前韧性值</param>
        /// <param name="max">新的韧性最大值</param>
        public void SetToughnessValue(int current, int max)
        {
            // 调用状态对象的内部方法更新值（保证状态一致性）
            _toughness.SetToughnessValue(current, max);
            // 触发韧性值变更事件
            BattleEntity.Context.GetEventBus().TriggerEvent(
                new ToughnessChangedEvent(
                    BattleEntity.Context, 
                    BattleEntity, 
                    _toughness.CurrentToughnessValue, 
                    _toughness.MaxToughnessVaue
                )
            );
        }

        /// <summary>
        /// 判断是否可扣除韧性（私有）
        /// 规则：1. 已破韧则不可扣除 → 2. 遍历所有扣除策略，只要有一个策略允许则返回可扣除
        /// </summary>
        /// <param name="reducer">扣除发起者</param>
        /// <param name="propertyType">元素属性类型</param>
        /// <param name="value">基础扣除值</param>
        /// <returns>true=可扣除，false=不可扣除</returns>
        private bool CanReduceToughness(IBattleEntityObject reducer, E_ElementType propertyType, int value)
        {
            // 已处于破韧状态，直接禁止扣除
            if (_toughness.IsBroken)
            {
                return false;
            }

            // 遍历所有扣除策略，只要有一个策略允许扣除，就返回true
            foreach (var reduceStrategy in _toughnessReduceStrategies)
            {
                if (reduceStrategy.CanReduceToughness(reducer, BattleEntity, propertyType, value))
                {
                    return true;
                }
            }

            // 所有策略都不允许，返回false
            return false;
        }

        /// <summary>
        /// 计算韧性扣除值（私有）
        /// 规则：叠加所有计算策略的返回值，得到最终扣除值
        /// </summary>
        /// <param name="reducer">扣除发起者</param>
        /// <param name="propertyType">元素属性类型</param>
        /// <param name="value">基础扣除值</param>
        /// <returns>最终要扣除的韧性值</returns>
        private int CalcToughness(IBattleEntityObject reducer, E_ElementType propertyType, int value)
        {
            var totalValue = 0;
            // 遍历所有计算策略，累加每个策略的计算结果
            foreach (var calcStrategy in _toughnessCalcStrategies)
            {
                totalValue += calcStrategy.CalcReduceToughness(reducer, BattleEntity, propertyType, value);
            }

            return totalValue;
        }

        /// <summary>
        /// 判断是否处于破韧状态
        /// 说明：破韧判定由ToughnessState内部维护（通常为当前韧性值≤0）
        /// </summary>
        /// <returns>true=已破韧，false=未破韧</returns>
        public bool IsToughnessBroken() => _toughness.IsBroken;

        /// <summary>
        /// 当前韧性值（只读属性）
        /// 说明：对外暴露当前韧性值，避免直接修改状态对象
        /// </summary>
        public int CurrentToughnessValue => _toughness.CurrentToughnessValue;

        /// <summary>
        /// 韧性最大值（只读属性）
        /// 说明：对外暴露韧性最大值，避免直接修改状态对象
        /// </summary>
        public int MaxToughnessVaue => _toughness.MaxToughnessVaue;

        /// <summary>
        /// 弱点属性列表（只读属性）
        /// 说明：对外暴露弱点属性，供伤害计算、UI显示等逻辑使用
        /// </summary>
        public List<E_ElementType> WeakPropertys => _toughness.WeakPropertys;
    }
}