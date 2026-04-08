using System;
using System.Collections.Generic;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;

namespace HotUpdate.Core.Task
{
    /// <summary>
    /// 任务接口
    /// </summary>
    public interface IQuest
    {
        QuestConfig.QuestItem QuestItem { get; }
        
        QuestData QuestData { get; }
        
        bool IsTracking { get; }
        
        event Action<int> OnQuestComplete;
        
        /// <summary>
        /// 接取该任务
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        void Accept();

        void CancelAccept();
    }
}
