using System;

namespace HotUpdate.Config.Quest
{
    /// <summary>
    /// 任务节点数据，表示单一任务中的单一节点
    /// </summary>
    [Serializable]
    public class QuestNodeData
    {
        // 节点唯一标识
        private int nodeId;               
        // 任务阶段
        private EQuestPhase phase;           
        // 通用进度（0~目标值），适用于大部分计数条件
        private int progress;                
        // 下一节点ID，若为-1则表示节点所处的任务完成
        private int nextNodeId;
        
        public event Action<QuestNodeData> OnDataChanged;

        public QuestNodeData(int nodeId, EQuestPhase phase, int progress, int nextNodeId)
        {
            this.nodeId = nodeId;
            this.phase = phase;
            this.progress = progress;
            this.nextNodeId = nextNodeId;
        }
        
        public int NodeId => nodeId;

        public EQuestPhase Phase
        {
            get => phase;
            set
            {
                phase = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public int Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnDataChanged?.Invoke(this);
            }
        }
        
        public int NextNodeId => nextNodeId;
    }
}
