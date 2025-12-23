using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 伤害结果
/// </summary>
public readonly struct DamageResult
{
    // 伤害来源
    public IBattleEntityObject Source { get; }

    //目标
    public IBattleEntityObject Target { get; }

    //最终伤害
    public int FinalDamage { get; }

    //伤害属性
    public E_ElementType ElementType { get; }

    //伤害类型
    public E_DamageType DamageType { get; }

    // 是否暴击
    public bool IsCrit { get; }

    public DamageResult(IBattleEntityObject source, IBattleEntityObject target, int finalDamage, E_ElementType elementType, E_DamageType damageType, bool isCrit)
    {
        Source = source;
        Target = target;
        FinalDamage = finalDamage;
        ElementType = elementType;
        DamageType = damageType;
        IsCrit = isCrit;
    }
}
