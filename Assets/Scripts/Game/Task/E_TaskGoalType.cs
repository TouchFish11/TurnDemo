using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务目标类型
/// </summary>
public enum E_TaskGoalType
{
    /// <summary>
    /// 和NPC对话
    /// </summary>
    TalkToNPC, 
    KillEnemy, // 击杀敌人
    CollectItem, // 采集物品
    ReachPosition, // 到达指定位置
    UseItem, // 使用物品
}
