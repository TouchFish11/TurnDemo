using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 属性加成类型
/// </summary>
public enum E_PropertyBonusType
{
    // 生命相关
    BuildHp,
    PercentHp,

    // 攻击相关
    BuildAtk,
    PercentAtk,

    // 防御相关
    BuildDef,
    PercentDef,

    // 速度相关
    BuildSpeed, 
    PercentSpeed,

    // 暴击相关
    BuildCrit,
    PercentCrit,

    // 爆伤相关
    BuildCritDmg, 
    PercentCritDmg,
}
