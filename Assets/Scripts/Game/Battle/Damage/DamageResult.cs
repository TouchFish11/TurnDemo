using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 伤害结果
/// </summary>
public readonly struct DamageResult
{
    /// <summary>
    /// 伤害来源
    /// </summary>
    public IBattleEntityObject Source { get; }

    /// <summary>
    /// 目标
    /// </summary>
    public IBattleEntityObject Target { get; }

    /// <summary>
    /// 最终伤害
    /// </summary>
    public int FinalDamage { get; }

    /// <summary>
    /// 伤害属性
    /// </summary>
    public E_ElementType ElementType { get; }

    /// <summary>
    /// 伤害类型
    /// </summary>
    public E_DamageType DamageType { get; }

    /// <summary>
    /// 是否暴击
    /// </summary>
    public bool IsCrit { get; }

    /// <summary>
    /// 削韧量
    /// </summary>
    public int ToughenValue { get; }

    public DamageResult(IBattleEntityObject source, IBattleEntityObject target, int finalDamage, E_ElementType elementType, E_DamageType damageType, bool isCrit, int toughenValue)
    {
        Source = source;
        Target = target;
        FinalDamage = finalDamage;
        ElementType = elementType;
        DamageType = damageType;
        IsCrit = isCrit;
        ToughenValue = toughenValue;
    }
}
