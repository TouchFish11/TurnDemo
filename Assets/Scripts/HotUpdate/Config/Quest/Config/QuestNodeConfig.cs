using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace HotUpdate.Config.Quest.Config
{
    /// <summary>
    /// 任务节点配置
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class QuestNodeConfig
    {
        // 任务节点唯一ID，当前任务内的唯一ID，和其它任务的任务节点ID不冲突
        public int nodeId;
        // 任务节点名称
        public string name;
        // 任务栏提示文本
        public string questTip;
        // 任务节点描述
        public string description;
        // 当前节点的最大进度，达到最大进度则视为完成，对应可以抽象为任务进度配置类，对应数据使用的任务进度类
        public int maxProgress;
        // 当前节点的完成条件类型，通过运行时工厂创建具体类
        public EQuestConditionType conditionType;
        // 当前节点的完成奖励，空列表则没有奖励，格式：物品ID,数量;物品ID2,数量...
        public string rewardItemIds;
        // 下一个任务节点ID，若没有则为-1，表明当前节点所处的任务结束，这里可拓展为列表存储当前节点可到的下一任务节点，支持任务分支，若列表无元素，则当前节点所处的任务结束
        public int nextNodeId;
        // 任务条件配置，可拓展为列表维护所有当前节点需完成的所有任务条件，支持单个阶段并行多个任务逻辑配置
        [FormerlySerializedAs("condition")] [SerializeReference] public QuestConditionConfig conditionConfig;
    }
}
