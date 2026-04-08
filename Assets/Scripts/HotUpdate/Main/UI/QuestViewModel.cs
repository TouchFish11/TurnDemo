using System;
using System.Collections.Generic;
using Core;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;

namespace HotUpdate.Main.UI
{
    /// <summary>
    /// 主界面和任务数据的ViewModel
    /// </summary>
    public class QuestViewModel
    {
        private List<QuestConfig.QuestItem> _questItems;
        private List<QuestData> _questDatas;
        
        public ReactiveProperty<string> QuestTitleName { get; } =  new();
        public ReactiveProperty<string> QuestTip { get; } = new();
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
                        if (nodeConfig == null) 
                            throw new NullReferenceException($"{nameof(QuestViewModel)}:{nameof(nodeConfig)} is null");
                        
                        QuestTitleName.Value = nodeConfig.name;
                        QuestTip.Value = nodeConfig.questTip;
                        QuestProgress.Value = $"{data.Progress}/{nodeConfig.maxProgress}";
                    };
                }
            }
            
            _questItems = questConfig.questItems;
            _questDatas = questDatas;
        }

        /// <summary>
        /// 刷新主界面任务栏UI
        /// </summary>
        /// <param name="questConfig"></param>
        /// <param name="nodeData"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void RefleshUI(QuestConfig questConfig, QuestNodeData nodeData)
        {
            if(questConfig == null)
                throw new ArgumentNullException($"{nameof(QuestViewModel)}:{nameof(questConfig)} is null");
            
            // 没有存在正在追踪的任务
            if (nodeData == null)
            {
                IsActiveQuestbar.Value = true;
                IsActiveQuestbar.Value = false;
                return;
            }

            IsActiveQuestbar.Value = true;
            var nodeConfig = GetNodeConfig(questConfig, nodeData.NodeId);
            if (nodeConfig == null) throw new NullReferenceException($"{nameof(nodeConfig)} is null");
            QuestTitleName.Value = nodeConfig.name;
            QuestTip.Value = nodeConfig.questTip;
            QuestProgress.Value = $"{nodeData.Progress}/{nodeConfig.maxProgress}";
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
