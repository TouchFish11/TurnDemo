using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可交互的
/// </summary>
public interface IInteractable
{
    NpcConfig NpcConfig { get; }

    /// <summary>
    /// 是否显示浮动文本
    /// </summary>
    public bool IsShowFloatingText { get; }

    /// <summary>
    /// 交互
    /// </summary>
    void OnInteract(IEntityObject entityObject);
}
