using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 韧性计算策略接口
/// </summary>
public interface IToughnessCalcStrategy
{
    /// <summary>
    /// 规则优先级
    /// 数值越大越先执行
    /// 属性名和ToughnessStrategyAttribute中的要一致
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 计算韧性值
    /// </summary>
    /// <param name="reducer"></param>
    /// <param name="target"></param>
    /// <param name="propertyType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    int CalcReduceToughness(IBattleEntityObject reducer, IBattleEntityObject target, E_ElementType propertyType, int value);
}
