using System.Collections.Generic;
using HotUpdate.Config.Quest;

namespace HotUpdate.Core.Task
{
    public interface IQuest
    {
        /// <summary>
        /// 接取该任务
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        void Accept();

        QuestConfig.QuestItem QuestItem { get; }
        QuestData QuestData { get; }

        void CancelAccept();
    }
}
