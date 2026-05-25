using System.Collections.Generic;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Quest;
using HotUpdate.Common.Config.Quest.Config;

namespace HotUpdate.Base.Manager
{
    public interface IQuestManager
    {
        /// <summary>
        /// 初始化任务对象，同时将新增玩家没有的任务数据，只会创建未完成的任务
        /// </summary>
        /// <param name="questConfig"></param>
        /// <param name="questCollection"></param>
        void InitQuests(QuestConfig questConfig, IQuestCollection questCollection);

        IEnumerable<IQuest> GetQuests();
        void AcceptQuest(int questId);
        void CancelQuest();
    }
}
