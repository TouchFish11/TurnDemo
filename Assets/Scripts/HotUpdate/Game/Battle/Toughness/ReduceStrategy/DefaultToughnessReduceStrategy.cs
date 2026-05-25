using HotUpdate.Base;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;

namespace HotUpdate.Game.Battle.Toughness.ReduceStrategy
{
    /// <summary>
    /// 默认韧性削减策略
    /// 仅处理基础的弱点属性判定，特殊情况的处理保留给更高优先级的策略
    /// </summary>
    [ToughnessStrategy(E_ToughnessStrategyType.ReduceJudge, 0)]
    public class DefaultToughnessReduceStrategy : IToughnessReduceStrategy
    {
        public int Priority { get; private set; }

        /// <summary>
        /// 判断是否可以削减韧性
        /// 仅根据弱点属性进行基础判定
        /// </summary>
        public bool CanReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value)
        {
            // 只有攻击属性命中目标弱点时才能削减韧性
            if (target.GetComponent<ToughnessComponent>().WeakPropertys.Contains(propertyType))
            {
                return true;
            }
            return false;
        }
    }
}