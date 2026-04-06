using System;
using System.Collections.Generic;
using HotUpdate.Core.Data;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务数据，表示单一任务
    /// </summary>
    [Serializable]
    public class QuestData : IData<QuestData>
    {
        // 任务唯一ID
        private int questId;
        // 任务阶段
        private EQuestPhase questPhase;
        // 所有当前任务所有节点的运行时数据，只要激活了节点，就会被添加到数据中
        private List<QuestNodeData> nodeDatas;
        // 是否正在追踪任务
        private bool isTracking;
        // 当前激活的节点，适用于线性任务
        private int curActiveNodeId;
        // 或者支持多激活节点：List<string> activeNodeIds
        // ...

        /// <summary>
        /// 任务唯一ID
        /// </summary>
        public int QuestId => questId;

        /// <summary>
        /// 任务阶段
        /// </summary>
        public EQuestPhase QuestPhase
        {
            get => questPhase;
            set
            {
                questPhase = value;
                OnDataChanged?.Invoke(this);
            }
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
                OnDataChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// 若正在追踪任务，则为任务节点ID，否则忽略该属性
        /// </summary>
        public int CurActiveNodeId
        {
            get => curActiveNodeId;
            set
            {
                curActiveNodeId = value;
                OnDataChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// 添加节点任务
        /// </summary>
        /// <param name="nodeData"></param>
        public void AddNodeData(QuestNodeData nodeData)
        {
            nodeDatas.Add(nodeData);
            OnDataChanged?.Invoke(this);
        }

        public IEnumerable<QuestNodeData> GetNodeDatas()
        {
            foreach (var questNodeData in nodeDatas)
            {
                yield return questNodeData;
            }
        }

        public event Action<QuestData> OnDataChanged;
    }
}
