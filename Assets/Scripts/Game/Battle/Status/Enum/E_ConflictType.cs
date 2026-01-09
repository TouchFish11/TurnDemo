using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冲出类型
/// </summary>
public enum E_ConflictType : byte
{
    /// <summary>
    /// 叠加
    /// </summary>
    Add = 1,
    /// <summary>
    /// 独立
    /// </summary>
    Lonel,
    /// <summary>
    /// 覆盖
    /// </summary>
    Cover,
}
