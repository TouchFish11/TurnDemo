using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Property;
using HotUpdate.Base.Battle.Toughness;

namespace HotUpdate.Game.Battle.Toughness.ReduceStrategy
{
    /// <summary>
    /// Ĭ��������������
    /// ���ڶ�Ӧ���㣬�����������ԣ����㱣�����⣩
    /// </summary>
    [ToughnessStrategy(E_ToughnessStrategyType.ReduceJudge, 0)]
    public class DefaultToughnessReduceStrategy : IToughnessReduceStrategy
    {
        public int Priority { get; private set; }

        public bool CanReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value)
        {
            if (target.GetComponent<ToughnessComponent>().WeakPropertys.Contains(propertyType))
            {
                return true;
            }
            return false;
        }
    }
}
