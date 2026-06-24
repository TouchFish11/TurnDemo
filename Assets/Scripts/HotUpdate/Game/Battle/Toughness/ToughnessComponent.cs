using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Toughness
{
    /// <summary>
    /// 韧性组件
    /// 功能：管理战斗实体（怪物/角色）的韧性系统核心逻辑，包括韧性初始化、韧性值计算/扣除、破韧判断、策略管理等
    /// 依赖：BattleComponent（战斗组件基类）、IToughnessComponent（韧性组件接口）、策略模式（扣除/计算策略）、事件总线（状态变更通知）
    /// </summary>
    [ComponentId(typeof(ToughnessComponent))] // 标记组件唯一标识，用于组件注册和获取
    [ComponentCore(typeof(ToughnessComponentCore))]
    public class ToughnessComponent : BattleComponent
    {
        private ToughnessComponentCore _toughnessComponentCore;

        protected override void OnBattleInit()
        {
            _toughnessComponentCore = (ToughnessComponentCore)ComponentCore;
        }
        
        /// <summary>
        /// 添加韧性扣除策略
        /// 说明：添加后自动按优先级重新排序，保证高优先级策略先执行
        /// </summary>
        /// <param name="reduceStrategy">待添加的扣除策略实例</param>
        public void AddToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy)
        {
            _toughnessComponentCore.AddToughnessReduceStrategy(reduceStrategy);
        }

        /// <summary>
        /// 移除韧性扣除策略
        /// 说明：移除后自动重新排序，保证策略执行顺序正确
        /// </summary>
        /// <param name="reduceStrategy">待移除的扣除策略实例</param>
        public void RemoveToughnessReduceStrategy(IToughnessReduceStrategy reduceStrategy)
        {
            _toughnessComponentCore.RemoveToughnessReduceStrategy(reduceStrategy);
        }
        
        /// <summary>
        /// 添加韧性计算策略
        /// 说明：添加后自动按优先级重新排序，保证高优先级策略先执行
        /// </summary>
        /// <param name="calcStrategy">待添加的计算策略实例</param>
        public void AddToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy)
        {
            _toughnessComponentCore.AddToughnessCalcStrategy(calcStrategy);
        }

        /// <summary>
        /// 移除韧性计算策略
        /// 说明：移除后自动重新排序，保证策略执行顺序正确
        /// </summary>
        /// <param name="calcStrategy">待移除的计算策略实例</param>
        public void RemoveToughnessCalcStrategy(IToughnessCalcStrategy calcStrategy)
        {
            _toughnessComponentCore.RemoveToughnessCalcStrategy(calcStrategy);
        }
        
        /// <summary>
        /// 扣除韧性值（核心方法）
        /// 流程：1. 判断是否可扣除 → 2. 计算最终扣除值 → 3. 更新韧性值 → 4. 触发状态变更事件 → 5. 判断是否破韧并触发破韧事件
        /// </summary>
        /// <param name="reducer">扣除韧性的发起者（如攻击方角色/技能）</param>
        /// <param name="propertyType">触发扣除的属性类型</param>
        /// <param name="resilienceValue"></param>
        /// <param name="skillId"></param>
        public void ReduceToughness(IBattleEntityObject reducer, E_ElementType propertyType, int resilienceValue, int skillId)
        {
            _toughnessComponentCore.ReduceToughness(reducer, propertyType, resilienceValue, skillId);
        }

        /// <summary>
        /// 设置韧性值（对外暴露的可控接口）
        /// 说明：修改韧性值后会主动触发状态变更事件，保证外部数据同步
        /// </summary>
        /// <param name="current">新的当前韧性值</param>
        /// <param name="max">新的韧性最大值</param>
        public void SetToughnessValue(int current, int max)
        {
            _toughnessComponentCore.SetToughnessValue(current, max);
        }
        
        protected override void OnBattleDestroy()
        {
            _toughnessComponentCore = null;
        }

        /// <summary>
        /// 判断是否处于破韧状态
        /// 说明：破韧判定由ToughnessState内部维护（通常为当前韧性值≤0）
        /// </summary>
        /// <returns>true=已破韧，false=未破韧</returns>
        public bool IsToughnessBroken()
        {
            return _toughnessComponentCore.IsToughnessBroken();
        }

        /// <summary>
        /// 当前韧性值（只读属性）
        /// 说明：对外暴露当前韧性值，避免直接修改状态对象
        /// </summary>
        public int CurrentToughnessValue => _toughnessComponentCore.CurrentToughnessValue;

        /// <summary>
        /// 韧性最大值（只读属性）
        /// 说明：对外暴露韧性最大值，避免直接修改状态对象
        /// </summary>
        public int MaxToughnessVaue => _toughnessComponentCore.MaxToughnessVaue;

        /// <summary>
        /// 弱点属性列表（只读属性）
        /// 说明：对外暴露弱点属性，供伤害计算、UI显示等逻辑使用
        /// </summary>
        public List<E_ElementType> WeakPropertys => _toughnessComponentCore.WeakPropertys;
    }
}