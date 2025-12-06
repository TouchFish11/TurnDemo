using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC配置
/// </summary>
[CreateAssetMenu()]
public class NpcConfig : ScriptableObject
{
    /// <summary>
    /// NPC名称
    /// </summary>
    public string npcName;

    /// <summary>
    /// Npc身份
    /// </summary>
    public string npcIdentity;

    /// <summary>
    /// 对话起始ID
    /// </summary>
    public int dialogueId;
}
