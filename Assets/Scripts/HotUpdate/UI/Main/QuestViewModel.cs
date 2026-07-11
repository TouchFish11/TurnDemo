using System;
using System.Collections.Generic;
using Core.UI;
using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Config.Quest.Config;

namespace HotUpdate.UI.Main
{
    /// <summary>
    /// TODO：优化，不应该持有数据
    /// 主界面和任务数据的ViewModel
    /// </summary>
    public class QuestViewModel : IDisposable
    {
        private readonly Dictionary<int, Dictionary<int, QuestNodeConfig>> _nodeConfigs = new();
        private readonly Dictionary<int, Dictionary<int, QuestNodeData>> _nodeDatas = new();
        public ReactiveProperty<string> QuestTitleName { get; private set; } =  new();
        public ReactiveProperty<string> QuestTip { get; private set; } = new();
        public ReactiveProperty<string> QuestProgress { get; private set; } = new();
        public ReactiveProperty<bool> IsActiveQuestbar { get; private set; } = new();

        public QuestViewModel(QuestConfig questConfig, List<QuestData> questDatas)
        {
            // 缓存所有任务配置/数据节点
            CacheNodeConfigs(questConfig);
            CacheNodeDatas(questDatas);
            
            foreach (var questData in questDatas)
            {
                foreach (var nodeData in questData.GetNodeDatas())
                {
                    // 数据流向UI
                    nodeData.OnDataChanged += data =>
                    {
                        IsActiveQuestbar.Value = data.Phase == EQuestPhase.Processing;
                        var nodeConfig = _nodeConfigs[questData.QuestId][data.NodeId];
                        if (nodeConfig == null) 
                            throw new NullReferenceException($"{nameof(nodeConfig)} is null");
                        
                        QuestTitleName.Value = nodeConfig.name;
                        QuestTip.Value = nodeConfig.questTip;
                        QuestProgress.Value = $"{data.Progress}/{nodeConfig.maxProgress}";
                    };
                }
            }
        }

        /// <summary>
        /// 刷新主界面任务栏UI
        /// </summary>
        /// <param name="questData">正在追踪的任务的数据</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RefreshUI(QuestData questData)
        {
            // 没有存在正在追踪的任务
            if (questData == null)
            {
                QuestTitleName.Value = string.Empty;
                QuestTip.Value = string.Empty;
                QuestProgress.Value = string.Empty;
                IsActiveQuestbar.Value = false;
                return;
            }
            
            IsActiveQuestbar.Value = true;
            var nodeConfig = _nodeConfigs[questData.QuestId][questData.CurActiveNodeId];
            if (nodeConfig == null) 
                throw new NullReferenceException($"{nameof(nodeConfig)} is null");
            
            QuestTitleName.Value = nodeConfig.name;
            QuestTip.Value = nodeConfig.questTip;
            QuestProgress.Value = $"{_nodeDatas[questData.QuestId][questData.CurActiveNodeId].Progress}/{nodeConfig.maxProgress}";
        }

        /// <summary>
        /// 缓存所有节点配置数据
        /// </summary>
        /// <param name="questConfig"></param>
        private void CacheNodeConfigs(QuestConfig questConfig)
        {
            foreach (var questItem in questConfig.questItems)
            {
                // 单个任务对应其所有任务节点
                _nodeConfigs.Add(questItem.id, new Dictionary<int, QuestNodeConfig>());
                foreach (var nodeConfig in questItem.nodeConfigs)
                {
                    // 每个节点ID对应一个任务节点
                    _nodeConfigs[questItem.id].Add(nodeConfig.nodeId, nodeConfig);
                }
            }
        }
        
        /// <summary>
        /// 缓存所有节点数据
        /// </summary>
        /// <param name="questDatas"></param>
        private void CacheNodeDatas(List<QuestData> questDatas)
        {
            foreach (var questData in questDatas)
            {
                // 单个任务对应其所有任务节点
                _nodeDatas.Add(questData.QuestId, new Dictionary<int, QuestNodeData>());
                foreach (var nodeData in questData.GetNodeDatas())
                {
                    // 每个节点ID对应一个任务节点
                    _nodeDatas[questData.QuestId].Add(nodeData.NodeId, nodeData);
                }
            }
        }

        public void ResetViewModel()
        {
            QuestTitleName.Value = null;
            QuestTip.Value = null;
            QuestProgress.Value = null;
            IsActiveQuestbar.Value = false;
        }

        public void Dispose()
        {
            QuestTitleName.Dispose();
            QuestTip.Dispose();
            QuestProgress.Dispose();
            IsActiveQuestbar.Dispose();

            QuestTitleName = null;
            QuestTip = null;
            QuestProgress = null;
            IsActiveQuestbar = null;
        }
    }
}
