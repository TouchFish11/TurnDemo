using System.Collections.Generic;
using HotUpdate.Config.Quest;

namespace HotUpdate.Core.Task
{
    public interface IQuestCollection
    {
        bool TryGetValue(int id, out QuestData data);
        
        /// <summary>
        /// 尝试获取正在追踪的任务，存在时QuestData的curActiveNodeId不为默认值
        /// </summary>
        /// <returns></returns>
        bool TryGetTrackQuest(out QuestData data);
        
        List<QuestData> GetQuestDatas();
        
        void AddQuestData(QuestData data);
    }
}
