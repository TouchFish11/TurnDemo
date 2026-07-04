using HotUpdate.Base;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;

namespace HotUpdate.Game.Battle.Toughness.CalcStrategy
{
    /// <summary>
    /// 默认韧性计算策略
    /// </summary>
    [ToughnessStrategy(E_ToughnessStrategyType.ValueCalculate, 0)]
    public class DefaultToughnessCalcStrategy : IToughnessCalcStrategy
    {
        public int Priority { get; private set; }

        public int CalcReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value)
        {
            return value;
        }
    }
}