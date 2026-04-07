using System.Collections.Generic;
using HotUpdate.Config.Quest;

namespace HotUpdate.Core.Task
{
    public interface IQuestManager
    {
        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="questConfig"></param>
        /// <param name="questCollection"></param>
        void InitQuests(QuestConfig questConfig, IQuestCollection questCollection);

        IEnumerable<IQuest> GetQuests();
        void AcceptQuest(int questId);
        void CancelQuest();
    }
}
