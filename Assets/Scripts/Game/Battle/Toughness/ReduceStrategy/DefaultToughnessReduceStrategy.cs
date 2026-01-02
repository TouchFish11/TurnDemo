using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 默认韧性削减策略
/// 存在对应弱点，即可削减韧性（弱点保护除外）
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
