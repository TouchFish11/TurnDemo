using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能类型
/// </summary>
public enum E_SkillType : byte
{
    /// <summary>
    /// 怪物技能类型统一用该类型表示
    /// </summary>
    Monster,
    /// <summary>
    /// 普通攻击
    /// </summary>
    NormalAttack,
    /// <summary>
    /// 战技
    /// </summary>
    CombatSkill,
    /// <summary>
    /// 终结技
    /// </summary>
    UltimateSkill,
    /// <summary>
    /// 强化普攻
    /// </summary>
    EnhancedNormalAttack,
    /// <summary>
    /// 强化战技
    /// </summary>
    EnhancedCombatSkill,
}
