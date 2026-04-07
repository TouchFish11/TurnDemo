using System.Collections.Generic;
using Core.Pool;
using HotUpdate.Config.Quest;
using HotUpdate.Core.Task;
using HotUpdate.Task.Quest.Condition;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务对象，由任务节点构成任务逻辑。可被复用
    /// </summary>
    public class Quest : IQuest, IPoolData
    {
        private QuestConfig.QuestItem _questItem;
        private QuestData _questData;
        private readonly Dictionary<int, QuestNode> _questNodes = new();
        private QuestNode _currentNode;

        public QuestConfig.QuestItem QuestItem => _questItem;
        public QuestData QuestData => _questData;
        
        public Quest(QuestConfig.QuestItem questItem, QuestData questData)
        {
            _questItem = questItem;
            _questData = questData;
            foreach (var questDataNodeData in questData.GetNodeDatas())
            {
                var nodeConfig = _questItem.nodeConfigs.Find(config => config.nodeId == questDataNodeData.NodeId);
                var condition = QuestConditionFactory.CreateCondition(nodeConfig.conditionType);
                var questNode = new QuestNode(nodeConfig, questDataNodeData, condition);
                _questNodes.Add(questDataNodeData.NodeId, questNode);
            }

            CheckTrack();
        }

        /// <summary>
        /// 接取该任务
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        public void Accept()
        {
            foreach (var questNode in _questNodes.Values)
            {
                if(questNode.QuestNodeData.Phase == EQuestPhase.Complete) continue;
                questNode.OnComplete += SwitchNext;
                questNode.Active();
                _currentNode = questNode;
                break;
            }
        }

        public void CancelAccept()
        {
            if (_currentNode == null) return;
            _currentNode.Inactive();
            _questData.IsTracking = false;
            _currentNode = null;
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
                questNode.Active();
                _currentNode = questNode;
            }
        }

        /// <summary>
        /// 切换到下一个节点任务
        /// </summary>
        /// <param name="nextNodeId"></param>
        private void SwitchNext(int nextNodeId)
        {
            // 任务完成
            if (nextNodeId == QuestUtil.QUEST_NODE_END_ID)
            {
                _questData.IsTracking = false;
                _questData.CurActiveNodeId = QuestUtil.QUEST_NODE_END_ID;
                _questData.IsComplete = true;
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
            _questItem = null;
            _questData = null;
            _currentNode = null;
            _questNodes.Clear();
        }
    }
}
