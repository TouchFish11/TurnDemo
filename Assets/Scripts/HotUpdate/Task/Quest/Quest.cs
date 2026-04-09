using System;
using System.Collections.Generic;
using Core.Pool;
using HotUpdate.Common.Quest;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;
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
        
        public bool IsTracking => _questData.IsTracking;
        
        public event Action<int> OnQuestComplete;

        public Quest(QuestConfig.QuestItem questItem, QuestData questData)
        {
            _questItem = questItem;
            _questData = questData;
            foreach (var questDataNodeData in questData.GetNodeDatas())
            {
                var nodeConfig = _questItem.nodeConfigs.Find(config => config.nodeId == questDataNodeData.NodeId);
                var condition = QuestConditionFactory.CreateCondition(nodeConfig.conditionType, nodeConfig.conditionConfig);
                var questNode = new QuestNode(nodeConfig, questDataNodeData, condition);
                questNode.OnComplete += SwitchNext;
                _questNodes.Add(questDataNodeData.NodeId, questNode);
            }

            // 主动检查自己是否被追踪
            CheckTrack();
        }

        /// <summary>
        /// 接取该任务，任务将被追踪
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        public void Accept()
        {
            QuestNode currentNode = null;
            // 存在追踪的任务，直接获取节点
            if (_questNodes.TryGetValue(_questData.CurActiveNodeId, out var node))
            {
                currentNode = node;
            }
            // 否则就找到当前任务的第一个未完成的节点
            else
            {
                foreach (var questNode in _questNodes.Values)
                {
                    if(questNode.QuestNodeData.Phase == EQuestPhase.Complete) continue;
                    currentNode = questNode;
                    break;
                }
            }
            
            if(currentNode == null)
                throw new NullReferenceException($"Quest {_questData.CurActiveNodeId} is not found.");
            
            currentNode.Active();
            _questData.IsTracking = true;
            _questData.CurActiveNodeId = currentNode.QuestNodeData.NodeId;
            _currentNode = currentNode;
        }

        public void CancelAccept()
        {
            if (_currentNode == null) return;
            _currentNode.Inactive();
            _questData.IsTracking = false;
            _questData.CurActiveNodeId = QuestUtil.QUEST_INACTIVE_NODE_ID;
            _currentNode = null;
        }

        /// <summary>
        /// 检查该任务是否被追踪，若为追踪状态则激活对应任务节点，监听任务事件
        /// </summary>
        private void CheckTrack()
        {
            // 没有正在追踪的任务
            if(!_questData.IsTracking) return;

            if (!_questNodes.TryGetValue(_questData.CurActiveNodeId, out var questNode))
                throw new KeyNotFoundException($"{nameof(Quest)}.{nameof(CheckTrack)}: {_questData.CurActiveNodeId} is not found.)");
            
            questNode.Active();
            _currentNode = questNode;
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
                _currentNode.QuestNodeData.SelNextNodeId = QuestUtil.QUEST_NODE_END_ID;
                _questData.IsComplete = true;
                OnQuestComplete?.Invoke(_questItem.id);
                OnQuestComplete = null;
                _currentNode.Dispose();
                return;
            }

            if (!_questNodes.TryGetValue(nextNodeId, out var questNode))
                throw new KeyNotFoundException($"{nameof(Quest)}.{nameof(SwitchNext)}: {nextNodeId} is not found.");
            
            _questData.CurActiveNodeId = nextNodeId;
            // 默认选择下一个节点，可拓展为根据玩家选择的分支记录对应的ID
            _currentNode.QuestNodeData.SelNextNodeId = nextNodeId;
            questNode.Active();
            _currentNode = questNode;
        }

        void IPoolData.ResetData()
        {
            _questItem = null;
            _questData = null;
            _currentNode = null;
            foreach (var nodesValue in _questNodes.Values)
            {
                nodesValue.Dispose();
            }
            _questNodes.Clear();
        }
    }
}
