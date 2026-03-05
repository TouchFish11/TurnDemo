using HotUpdate.Battle.Object;
using HotUpdate.Battle.Property;

namespace HotUpdate.Battle.Toughness.CalcStrategy
{
    /// <summary>
    /// Ĭ�����Լ������
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
