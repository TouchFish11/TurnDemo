using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
