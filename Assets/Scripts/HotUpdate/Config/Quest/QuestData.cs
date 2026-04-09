using System;
using System.Collections.Generic;
using Core.Log;
using Newtonsoft.Json;

namespace HotUpdate.Config.Quest
{
    /// <summary>
    /// 任务数据，表示单一任务，只要接取了任务就会存在任务数据，即使又取消接取，数据不会被移除
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class QuestData
    {
        // 任务唯一ID
        [JsonProperty] private int questId;
        // 是否完成，当该任务的所有节点都完成时，为true，否则为false
        [JsonProperty] private bool isComplete;
        // 所有当前任务所有节点的运行时数据，只要激活了节点，就会被添加到数据中
        [JsonProperty] private List<QuestNodeData> nodeDatas;
        // 是否正在追踪任务
        [JsonProperty] private bool isTracking;
        // 当前激活的节点
        [JsonProperty] private int curActiveNodeId;

        public QuestData(int questId, List<QuestNodeData> nodeDatas)
        {
            this.questId = questId;
            this.nodeDatas = nodeDatas;
            curActiveNodeId = -1;
        }
        
        /// <summary>
        /// 任务唯一ID
        /// </summary>
        public int QuestId => questId;

        /// <summary>
        /// 是否完成，当该任务的所有节点都完成时，为true，否则为false
        /// </summary>
        public bool IsComplete
        {
            get => isComplete;
            set => isComplete = value;
        }

        /// <summary>
        /// 是否正在追踪
        /// </summary>
        public bool IsTracking
        {
            get => isTracking;
            set
            {
                isTracking = value;
                LogManager.Log($"任务：{QuestId}，是否追踪：{isTracking}");
            }
        }

        /// <summary>
        /// 若正在追踪任务，则为任务节点ID，否则为-1
        /// </summary>
        public int CurActiveNodeId
        {
            get => curActiveNodeId;
            set => curActiveNodeId = value;
        }

        /// <summary>
        /// 添加节点任务
        /// </summary>
        /// <param name="nodeData"></param>
        public void AddNodeData(QuestNodeData nodeData)
        {
            nodeDatas.Add(nodeData);
        }

        public IEnumerable<QuestNodeData> GetNodeDatas()
        {
            foreach (var questNodeData in nodeDatas)
            {
                yield return questNodeData;
            }
        }

        /// <summary>
        /// 根据节点ID获取任务节点
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public QuestNodeData GetNodeData(int nodeId)
        {
            return nodeDatas.Find(data => data.NodeId == nodeId);
        }
    }
}
