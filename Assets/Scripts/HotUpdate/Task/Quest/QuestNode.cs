using System;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务节点对象
    /// </summary>
    public class QuestNode : IDisposable
    {
        // 任务节点配置
        private QuestNodeConfig _questNodeConfig;
        // 任务节点数据
        private QuestNodeData _questNodeData;
        // 任务条件
        private IQuestCondition _questCondition;
        
        public QuestNodeData QuestNodeData => _questNodeData;
        
        /// <summary>
        /// 任务节点完成事件，触发后自动置空
        /// </summary>
        public event Action<int> OnComplete;
        
        public QuestNode(QuestNodeConfig nodeConfig, QuestNodeData questNodeData, IQuestCondition questCondition)
        {
            _questNodeConfig = nodeConfig;
            _questNodeData = questNodeData;
            _questCondition = questCondition;
            _questCondition.OnComplete += Complete;
        }

        // 激活任务节点
        public void Active()
        {
            // 监听任务关心的事件
            _questCondition.Enable(_questNodeData);
            _questNodeData.Phase = EQuestPhase.Processing;
        }

        // 失活任务节点
        public void Inactive()
        {
            // 取消监听任务关心的事件
            _questCondition.Disable();
            if (_questNodeData.Phase == EQuestPhase.Processing)
            {
                _questNodeData.Phase = EQuestPhase.NoReceive;
            }
        }

        // 完成任务节点
        private void Complete()
        {
            // 更新阶段
            _questNodeData.Phase = EQuestPhase.Complete;
            // 先失活当前节点
            Inactive();
            OnComplete?.Invoke(_questNodeData.NextNodeId);
            OnComplete = null;
        }

        public void Dispose()
        {
            _questNodeConfig = null;
            _questNodeData = null;
            (_questCondition as IDisposable)?.Dispose();
            _questCondition = null;
            OnComplete = null;
        }
    }
}
