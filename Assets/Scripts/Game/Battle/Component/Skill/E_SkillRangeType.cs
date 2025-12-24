using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能范围类型
/// </summary>
public enum E_SkillRangeType : byte
{
    /// <summary>
    /// 单体
    /// </summary>
    Single = 1,
    /// <summary>
    /// 扩散
    /// </summary>
    Diffusion,
    /// <summary>
    /// 全体
    /// </summary>
    All,
}
