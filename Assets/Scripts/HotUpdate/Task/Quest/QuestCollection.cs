using System.Collections.Generic;
using Core.Collection;
using HotUpdate.Config.Quest;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Quest
{
    /// <summary>
    /// 任务集合
    /// </summary>
    public class QuestCollection : Collection<int, QuestData>, IQuestCollection
    {
        /// <summary>
        /// 尝试获取正在追踪的任务
        /// </summary>
        /// <returns></returns>
        public bool TryGetTrackQuest(out QuestData data)
        {
            foreach (var questData in Values)
            {
                if (!questData.IsTracking) continue;
                data = questData;
                return true;
            }
            data = null;
            return false;
        }

        public List<QuestData> GetQuestDatas()
        {
            List<QuestData> questDatas = new();
            foreach (var data in Values)
            {
                questDatas.Add(data);
            }
            return questDatas;
        }
    }
}
