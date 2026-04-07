using System.Collections.Generic;
using HotUpdate.Config.Quest;

namespace HotUpdate.Core.Task
{
    public interface IQuestCollection
    {
        bool TryGetValue(int id, out QuestData data);
        
        /// <summary>
        /// 是否包含正在追踪的任务
        /// </summary>
        /// <returns></returns>
        bool TryGetTrackQuest(out QuestData data);
        
        List<QuestData> GetQuestDatas();
    }
}
