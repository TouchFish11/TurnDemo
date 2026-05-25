using System;

namespace HotUpdate.Game.Battle.Toughness
{
    /// <summary>
    /// 韧性策略类型枚举
    /// </summary>
    public enum E_ToughnessStrategyType
    {
        /// <summary>
        /// 韧性减少判定策略
        /// </summary>
        ReduceJudge,

        /// <summary>
        /// 韧性值计算策略
        /// </summary>
        ValueCalculate
    }

    /// <summary>
    /// 韧性策略特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ToughnessStrategyAttribute : Attribute
    {
        /// <summary>
        /// 策略类型
        /// 区分判定策略和数值计算策略
        /// </summary>
        public E_ToughnessStrategyType StrategyType { get; }

        /// <summary>
        /// 策略优先级
        /// 数值越小越先执行
        /// </summary>
        public int Priority { get; }

        public ToughnessStrategyAttribute(E_ToughnessStrategyType strategyType, int priority)
        {
            StrategyType = strategyType;
            Priority = priority;
        }
    }
}