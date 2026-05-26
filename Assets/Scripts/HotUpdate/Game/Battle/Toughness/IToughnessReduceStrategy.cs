using HotUpdate.Base;

namespace HotUpdate.Game.Battle.Toughness
{
    /// <summary>
    /// 韧性削减判定策略接口
    /// </summary>
    public interface IToughnessReduceStrategy
    {
        /// <summary>
        /// 策略优先级
        /// 数值越小越先执行
        /// 需与ToughnessStrategyAttribute中的优先级保持一致
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 判断是否可以削减韧性
        /// </summary>
        /// <param name="reducer">削减方（攻击方）</param>
        /// <param name="target">目标方（受击方）</param>
        /// <param name="propertyType">元素属性类型</param>
        /// <param name="value">基础削减值</param>
        /// <returns>true：可以削减；false：不可以削减</returns>
        bool CanReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value);
    }
}