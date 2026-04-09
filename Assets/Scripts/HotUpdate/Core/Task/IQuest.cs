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
        
        bool IsTracking { get; }
        
        /// <summary>
        /// 任务完成事件回调，传递完成的任务ID，执行后会置空
        /// </summary>
        event Action<int> OnQuestComplete;
        
        /// <summary>
        /// 接取该任务
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        void Accept();

        /// <summary>
        /// 取消接取当前追踪的任务
        /// </summary>
        void CancelAccept();
    }
}
