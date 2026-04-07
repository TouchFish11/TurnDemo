using System;
using UnityEngine;

namespace HotUpdate.Config.Quest
{
    /// <summary>
    /// 任务节点配置
    /// </summary>
    [Serializable]
    public class QuestNodeConfig
    {
        // 任务节点唯一ID，当前任务内的唯一ID，和其它任务的任务节点ID不冲突
        public int nodeId;
        // 任务节点名称
        public string name;
        // 任务节点描述
        public string description;
        // 当前节点的最大进度，达到最大进度则视为完成
        public int maxProgress;
        // 当前节点的完成条件类型，通过运行时工厂创建具体类
        public EQuestConditionType conditionType;
        // 当前节点的完成奖励，空列表则没有奖励，格式：物品ID,数量;物品ID2,数量
        public string rewardItemIds;
        // 下一个任务节点ID，若没有则为-1
        public int nextNodeId;
        // 任务条件
        [SerializeReference] public QuestCondition condition;
    }
}
