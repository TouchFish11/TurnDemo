using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 韧性相关策略枚举
/// </summary>
public enum E_ToughnessStrategyType
{
    /// <summary>
    /// 能否削减判定规则
    /// </summary>
    ReduceJudge,

    /// <summary>
    /// 削减量计算规则
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
    /// 区分判定规则和数值规则
    /// </summary>
    public E_ToughnessStrategyType StrategyType { get; }

    /// <summary>
    /// 规则优先级
    /// 数值越大越先执行
    /// </summary>
    public int Priority { get; }

    public ToughnessStrategyAttribute(E_ToughnessStrategyType strategyType, int priority)
    {
        StrategyType = strategyType;
        Priority = priority;
    }
}
