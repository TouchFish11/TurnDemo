using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务内容类型
/// </summary>
public enum E_TaskContentType : byte
{
    /// <summary>
    /// 对话
    /// </summary>
    Dialogue = 1,
    /// <summary>
    /// 战斗
    /// </summary>
    Battle,
    /// <summary>
    /// 其它
    /// </summary>
    Other,
}
