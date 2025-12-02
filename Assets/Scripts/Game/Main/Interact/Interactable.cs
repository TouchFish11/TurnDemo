using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可交互的
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// NPC名称
    /// </summary>
    string NpcName {  get; }

    /// <summary>
    /// 是否可交互
    /// </summary>
    public bool IsInteractable { get; }

    /// <summary>
    /// 交互
    /// </summary>
    void OnInteract(IEntityObject entityObject);
}
