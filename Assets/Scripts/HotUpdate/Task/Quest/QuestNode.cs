using System;
using System.Collections.Generic;
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
        // 任务条件，可拓展为列表维护所有当前节点需完成的所有任务条件，支持单个阶段并行多个任务逻辑
        private IQuestCondition _questCondition;
        //private List<IQuestCondition> _questConditions = new();
        
        public QuestNodeData QuestNodeData => _questNodeData;
        
        /// <summary>
        /// 任务节点完成事件，传递下一个任务节点ID，触发后自动置空，外部无需-=
        /// </summary>
        public event Action<int> OnComplete;
        
        public QuestNode(QuestNodeConfig nodeConfig, QuestNodeData questNodeData, IQuestCondition questCondition)
        {
            _questNodeConfig = nodeConfig;
            _questNodeData = questNodeData;
            _questCondition = questCondition;
            _questCondition.OnProgressChanged += OnProgressChanged;
        }

        // 激活任务节点
        public void Active()
        {
            // 监听任务关心的事件
            _questCondition.Enable();
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

        /// <summary>
        /// 进度变化事件回调
        /// </summary>
        /// <param name="delta"></param>
        private void OnProgressChanged(int delta)
        {
            // 更新进度
            _questNodeData.Progress += delta;
            if (_questNodeData.Progress != _questNodeConfig.maxProgress) return;
            
            // 更新阶段
            _questNodeData.Phase = EQuestPhase.Complete;
            // 失活当前节点
            Inactive();
            // 调用当前节点完成回调，通知外部激活下一个任务节点
            OnComplete?.Invoke(_questNodeConfig.nextNodeId);
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
