using System.Collections.Generic;
using Core.Pool;
using HotUpdate.Task.Quest.Condition;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务对象，由任务节点构成任务逻辑。可被复用
    /// </summary>
    public class Quest : IPoolData
    {
        private QuestConfig _questConfig;
        private QuestData _questData;
        private readonly Dictionary<int, QuestNode> _questNodes = new();
        private QuestNode _currentNode;
        
        public Quest(QuestConfig questConfig, QuestData questData)
        {
            _questConfig = questConfig;
            _questData = questData;
            foreach (var questDataNodeData in questData.GetNodeDatas())
            {
                var condition = new TalkCondition();   // TODO:通过工厂获取
                var questNode = new QuestNode(questDataNodeData, condition);
                _questNodes.Add(questDataNodeData.NodeId, questNode);
            }

            CheckTrack();
        }

        public void Active(int nodeId)
        {
            if(!_questNodes.TryGetValue(nodeId, out var node)) 
                throw new KeyNotFoundException($"{nameof(Quest)}.{nameof(Active)}: {nodeId} is not found.");
            
            node.OnComplete += SwitchNext;
            node.Active();
            _currentNode = node;
        }

        /// <summary>
        /// 检查该任务是否被追踪
        /// </summary>
        private void CheckTrack()
        {
            // 没有正在追踪的任务
            if(!_questData.IsTracking) return;

            if (_questNodes.TryGetValue(_questData.CurActiveNodeId, out var questNode))
            {
                questNode.OnComplete += SwitchNext;
                _currentNode = questNode;
            }
        }

        /// <summary>
        /// 切换到下一个任务
        /// </summary>
        /// <param name="nextNodeId"></param>
        private void SwitchNext(int nextNodeId)
        {
            _currentNode.Inactive();
            // 任务完成
            if (nextNodeId == QuestUtil.QUEST_NODE_END_ID)
            {
                _questData.IsTracking = false;
                _questData.CurActiveNodeId = QuestUtil.QUEST_NODE_END_ID;
                _questData.QuestPhase = EQuestPhase.Complete;
                return;
            }

            if (!_questNodes.TryGetValue(nextNodeId, out var questNode))
                throw new KeyNotFoundException($"{nameof(Quest)}.{nameof(SwitchNext)}: {nextNodeId} is not found.");
            
            questNode.OnComplete += SwitchNext;
            questNode.Active();
            _currentNode = questNode;
        }

        public void ResetData()
        {
            _questConfig = null;
            _questData = null;
            _currentNode = null;
            _questNodes.Clear();
        }
    }
}
