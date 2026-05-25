using System.Collections.Generic;
using HotUpdate.Common.Config.Quest;

namespace HotUpdate.Base.Collection
{
    public interface IQuestCollection
    {
        bool TryGetValue(int id, out QuestData data);
        
        /// <summary>
        /// 尝试获取正在追踪的任务，存在时QuestData的curActiveNodeId不为默认值
        /// </summary>
        /// <returns></returns>
        bool TryGetTrackQuest(out QuestData data);
        
        /// <summary>
        /// 获取所有的任务数据，内部会new一个列表将缓存内容返回给外部
        /// </summary>
        /// <returns></returns>
        List<QuestData> GetQuestDatas();
        
        void AddQuestData(QuestData data);
    }
}
