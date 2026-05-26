using HotUpdate.Base;

namespace HotUpdate.Game.Battle.Toughness
{
    public interface IToughnessCalcStrategy
    {
        /// <summary>
        /// 策略优先级
        /// 数值越小越先执行
        /// 需与ToughnessStrategyAttribute中的优先级保持一致
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 计算韧性削减值
        /// </summary>
        /// <param name="reducer">削减方（攻击方）</param>
        /// <param name="target">目标方（受击方）</param>
        /// <param name="propertyType">元素属性类型</param>
        /// <param name="value">基础削减值</param>
        /// <returns>最终削减值</returns>
        int CalcReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value);
    }
}