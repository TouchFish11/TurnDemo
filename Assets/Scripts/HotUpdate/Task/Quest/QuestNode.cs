using System;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务节点对象
    /// </summary>
    [Serializable]
    public class QuestNode
    {
        // 任务节点数据
        private readonly QuestNodeData _questNodeData;
        // 任务条件
        private readonly QuestCondition _questCondition;
        // 任务节点完成事件
        public event Action<int> OnComplete;
        
        public QuestNode(QuestNodeData questNodeData, QuestCondition questCondition)
        {
            _questNodeData = questNodeData;
            _questCondition = questCondition;
            _questCondition.OnComplete += Complete;
        }

        // 激活任务节点
        public void Active()
        {
            // 监听任务关心的事件
            _questCondition.OnStart(this);
            _questNodeData.Phase = EQuestPhase.Processing;
        }

        // 失活任务节点
        public void Inactive()
        {
            // 取消监听任务关心的事件
            _questCondition.OnEnd();
            _questNodeData.Phase = EQuestPhase.NoReceive;
        }

        // 完成任务节点
        private void Complete()
        {
            // 更新阶段
            _questNodeData.Phase = EQuestPhase.Complete;
            // 先失活当前节点
            Inactive();
            // 接取下一个节点任务(若有)
            if (_questNodeData.NextNodeId == -1)
                return;
            
            OnComplete?.Invoke(_questNodeData.NextNodeId);
            OnComplete = null;
        }
    }
}
