using System;
using Newtonsoft.Json;

namespace HotUpdate.Common.Config.Quest
{
    /// <summary>
    /// 任务节点数据，表示单一任务中的单一节点数据，一个任务可以由多个节点数据构成
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class QuestNodeData
    {
        // 节点唯一标识
        [JsonProperty] private int nodeId;               
        // 任务阶段
        [JsonProperty] private EQuestPhase phase;           
        // 通用进度（0~目标值），适用于大部分计数条件，若需复杂进度，可抽象为任务进度类
        [JsonProperty] private int progress;     
        // 玩家已选择的下一节点ID，用于分支节点的记录，-1则没有下一个节点
        [JsonProperty] private int selNextNodeId;
        
        public event Action<QuestNodeData> OnDataChanged;

        public QuestNodeData(int nodeId, EQuestPhase phase, int progress)
        {
            this.nodeId = nodeId;
            this.phase = phase;
            this.progress = progress;
        }
        
        /// <summary>
        /// 节点唯一标识
        /// </summary>
        public int NodeId => nodeId;

        /// <summary>
        /// 任务阶段
        /// </summary>
        public EQuestPhase Phase
        {
            get => phase;
            set
            {
                phase = value;
                OnDataChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// 通用进度（0~目标值），适用于大部分计数条件，若需复杂进度，可抽象为任务进度类
        /// </summary>
        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnDataChanged?.Invoke(this);
            }
        }
        
        /// <summary>
        /// 玩家已选择的下一节点ID，用于分支节点的记录，-1则没有下一个节点
        /// </summary>
        public int SelNextNodeId
        {
            get => selNextNodeId;
            set
            {
                selNextNodeId = value;
                OnDataChanged?.Invoke(this);
            }
        }
    }
}
