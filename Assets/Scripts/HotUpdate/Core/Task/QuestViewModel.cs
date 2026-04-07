using System;
using System.Collections.Generic;
using Core;
using HotUpdate.Config.Quest;

namespace HotUpdate.Core.Task
{
    /// <summary>
    /// 任务界面ViewModel
    /// </summary>
    public class QuestViewModel
    {
        private List<QuestConfig.QuestItem> _questItems;
        private List<QuestData> _questDatas;
        
        public ReactiveProperty<string> QuestTitleName { get; } =  new();
        public ReactiveProperty<string> QuestDescription { get; } = new();
        public ReactiveProperty<string> QuestProgress { get; } = new();
        public ReactiveProperty<bool> IsActiveQuestbar { get; } = new();

        public QuestViewModel(QuestConfig questConfig, List<QuestData> questDatas)
        {
            foreach (var questData in questDatas)
            {
                foreach (var nodeData in questData.GetNodeDatas())
                {
                    // 数据流向UI
                    nodeData.OnDataChanged += data =>
                    {
                        IsActiveQuestbar.Value = data.Phase == EQuestPhase.Processing;
                        var nodeConfig = GetNodeConfig(questConfig, data.NodeId);
                        if (nodeConfig == null) throw new NullReferenceException($"{nameof(nodeConfig)} is null");
                        QuestTitleName.Value = nodeConfig.name;
                        QuestDescription.Value = nodeConfig.description;
                        QuestProgress.Value = $"{data.Progress}/{nodeConfig.maxProgress}";
                    };
                }
            }
            
            _questItems = questConfig.questItems;
            _questDatas = questDatas;
        }

        /// <summary>
        /// 获取对应ID的任务节点配置
        /// </summary>
        /// <param name="questConfig"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private static QuestNodeConfig GetNodeConfig(QuestConfig questConfig, int nodeId)
        {
            foreach (var questItem in questConfig.questItems)
            {
                var config = questItem.nodeConfigs.Find(config => config.nodeId == nodeId);
                if (config != null)
                {
                    return config;
                }
            }
            return null;
        }
    }
}
